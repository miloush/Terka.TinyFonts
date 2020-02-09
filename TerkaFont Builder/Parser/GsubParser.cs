namespace Terka.FontBuilder.Parser
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Media;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Substitution;
    using Terka.FontBuilder.Parser.Reflection;
    using Terka.FontBuilder.Parser.Reflection.Extensions;

    /// <summary>
    /// Corresponds to OT "GPOS LookupType".
    /// </summary>
    public enum SubstitutionLookupType : ushort
    {
        /// <summary>
        /// Corresponds to OT "Single substitution".
        /// </summary>
        Single = 1,

        /// <summary>
        /// Corresponds to OT "Multiple substitution".
        /// </summary>
        Multiple = 2,

        /// <summary>
        /// Corresponds to OT "Alternate substitution".
        /// </summary>
        Alternate = 3,

        /// <summary>
        /// Corresponds to OT "Ligature substitution".
        /// </summary>
        Ligature = 4,

        /// <summary>
        /// Corresponds to OT "Context substitution".
        /// </summary>
        Context = 5,

        /// <summary>
        /// Corresponds to OT "Chaining context substitution".
        /// </summary>
        ChainingContext = 6,

        /// <summary>
        /// Corresponds to OT "Extension substitution".
        /// </summary>
        ExtensionSubstitution = 7,

        /// <summary>
        /// Corresponds to OT "Reverse chaining context substitution".
        /// </summary>
        ReverseChainingContextSingle = 8
    }

    /// <summary>
    /// Parser of GSUB table.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Tohle se zdokumentuje pozdeji.")]
    public class GsubParser : TransformationParserBase
    {
        protected override dynamic GetFontTableHeader()
        {
            var headerType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.GSUBHeader");

            return Activator.CreateInstance(headerType).AccessNonPublic();
        }

        protected override dynamic GetFontTable(GlyphTypeface typeface)
        {
            var openTypeTagsType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.OpenTypeTags");

            dynamic font = this.GetOpenTypeFont(typeface);

            dynamic gsub = Enum.ToObject(openTypeTagsType, 1196643650u);
            return new AccessPrivateWrapper(font.GetFontTable(gsub));
        }

        protected override IGlyphTransformationTable GetSubstitutionTableBySubTableOffset(dynamic fontTable, int subTableOffset, int lookupTypeCode, LookupFlags lookupFlags)
        {
            switch ((SubstitutionLookupType)lookupTypeCode)
            {
                case SubstitutionLookupType.Single:
                    return this.ParseSingleSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.Multiple:
                    return this.ParseMultipleSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.Alternate:
                    break;
                case SubstitutionLookupType.Ligature:
                    return this.ParseLigatureSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.Context:
                    return this.ParseContextSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.ChainingContext:
                    return this.ParseChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.ReverseChainingContextSingle:
                    return this.ParseReverseChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case SubstitutionLookupType.ExtensionSubstitution:
                    return this.ParseExtensionLookupTable(subTableOffset, fontTable, lookupFlags);
                default:
                    throw new ArgumentOutOfRangeException("lookupTypeCode");
            }

            return null;
        }

        private IGlyphTransformationTable ParseSingleSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.SingleSubstitutionSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            switch (format)
            {
                case 1:
                    return new DeltaSubstitutionTable
                    {
                        Coverage = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped))),
                        GlyphIdDelta = (short)table.Format1DeltaGlyphId(fontTable.Wrapped),
                        LookupFlags = lookupFlags
                    };
                case 2:
                    var coverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));
                    return new SimpleReplacementSubstitutionTable
                    {
                        Coverage = coverage,
                        ReplacementGlyphIds =
                            coverage.CoveredGlyphIds.Keys
                                .Select(coverageIndex => (ushort)table.Format2SubstituteGlyphId(fontTable.Wrapped, coverageIndex)).ToList(),
                        LookupFlags = lookupFlags
                    };
                default:
                    throw new UnknownTableFormatException(type, format);
            }
        }

        private IGlyphTransformationTable ParseMultipleSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.MultipleSubstitutionSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            var coverageTable = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));

            var substitutionSequenceTables = this.GetEnumerableFromInternalList(
                () => (ushort)coverageTable.CoveredGlyphIds.Count, i => (dynamic)new AccessPrivateWrapper(table.Sequence(fontTable.Wrapped, i))).ToList();

            var sequences = substitutionSequenceTables
                .Select(
                    sequenceTable =>
                        this.GetEnumerableFromInternalList(
                            () => sequenceTable.GlyphCount(fontTable.Wrapped),
                            i => (ushort)sequenceTable.Glyph(fontTable.Wrapped, i))).ToList();

            return new MultipleSubstitutionTable
            {
                Coverage = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped))), 
                ReplacementSequences = sequences.ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseLigatureSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.LigatureSubstitutionSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            var ligatureSetTables = this.GetEnumerableFromInternalList(
                () => table.LigatureSetCount(fontTable.Wrapped), 
                i => (dynamic)new AccessPrivateWrapper(table.LigatureSet(fontTable.Wrapped, i))).ToList();

            var ligatures = ligatureSetTables
                .Select(
                    ligatureSetTable =>
                        this.GetEnumerableFromInternalList(
                            () => ligatureSetTable.LigatureCount(fontTable.Wrapped), 
                            i => (dynamic)new AccessPrivateWrapper(ligatureSetTable.Ligature(fontTable.Wrapped, i)))
                            .Select(
                                ligatureTable =>
                                    new Ligature
                                    {
                                        LigatureGlyphId = (ushort)ligatureTable.LigatureGlyph(fontTable.Wrapped), 
                                        ComponentGlyphIds = this.GetEnumerableFromInternalList(
                                            () => (ushort)(ligatureTable.ComponentCount(fontTable.Wrapped) - 1),
                                            i => (ushort)ligatureTable.Component(fontTable.Wrapped, (ushort)(i + 1)))
                                    }).ToList()).ToList();

            return new LigatureSubstitutionTable
            {
                Coverage = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped))),
                LigatureSets = ligatures.ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseReverseChainingSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ReverseChainingSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            var result = new ReverseChainingContextSubstitutionTable
            {
                Coverage = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.InputCoverage(fontTable.Wrapped)))
            };

            int currentOffset = table.offset + table.offsetBacktrackGlyphCount;

            ushort lookbackCount = table.GlyphCount(fontTable.Wrapped, currentOffset);
            currentOffset += 2;

            result.LookbackCoverages = this.GetEnumerableFromInternalList(
                () => lookbackCount,
                i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped, currentOffset)))
            ).ToList();

            currentOffset += 2 * lookbackCount;

            ushort lookaheadCount = table.GlyphCount(fontTable.Wrapped, currentOffset);
            currentOffset += 2;

            result.LookaheadCoverages = this.GetEnumerableFromInternalList(
                () => lookaheadCount,
                i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped, currentOffset)))
            ).ToList();

            currentOffset += 2 * lookaheadCount;

            ushort glyphCount = table.GlyphCount(fontTable.Wrapped, currentOffset);
            currentOffset += 2;

            result.SubstituteGlyphIds = this.GetEnumerableFromInternalList(
                () => glyphCount,
                i => (ushort)table.Glyph(fontTable.Wrapped, currentOffset)
            ).ToList();

            result.LookupFlags = lookupFlags;
            return result;
        }
    }
}