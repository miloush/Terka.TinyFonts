namespace Terka.FontBuilder.Parser
{
    using System;
    using System.Linq;
    using System.Windows.Media;
    
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Positioning;
    using Terka.FontBuilder.Parser.Reflection;
    using Terka.FontBuilder.Parser.Reflection.Extensions;

        /// <summary>
    /// Corresponds to OT "GPOS LookupType".
    /// </summary>
    public enum PositioningLookupType : ushort
    {
        /// <summary>
        /// Corresponds to OT "Single adjustment".
        /// </summary>
        Single = 1,

        /// <summary>
        /// Corresponds to OT "Pair substitution".
        /// </summary>
        Pair = 2,

        /// <summary>
        /// Corresponds to OT "Cursive attachment".
        /// </summary>
        Cursive = 3,

        /// <summary>
        /// Corresponds to OT "MarkToBase attachment".
        /// </summary>
        MarkToBase = 4,

        /// <summary>
        /// Corresponds to OT "MarkToLigature attachment".
        /// </summary>
        MarkToLigature = 5,

        /// <summary>
        /// Corresponds to OT "MarkToMark attachment".
        /// </summary>
        MarkToMark = 6,

        /// <summary>
        /// Corresponds to OT "Context positioning".
        /// </summary>
        Context = 7,

        /// <summary>
        /// Corresponds to OT "Chained context positioning".
        /// </summary>
        ChainingContext = 8,

        /// <summary>
        /// Corresponds to OT "Extension positioning".
        /// </summary>
        ExtensionPositioning = 9
    }

    [Flags]
    public enum ValueRecordFormatFlags : ushort
    {
        /// <summary>
        /// Corresponds to OT "XPlacement".
        /// </summary>
        XPlacementFlag = 0x0001,

        /// <summary>
        /// Corresponds to OT "YPlacement".
        /// </summary>
        YPlacementFlag = 0x0002,

        /// <summary>
        /// Corresponds to OT "XAdvance".
        /// </summary>
        XAdvanceFlag   = 0x0004,

        /// <summary>
        /// Corresponds to OT "XAdvance".
        /// </summary>
        YAdvanceFlag  = 0x0008,

        /// <summary>
        /// Corresponds to OT "XPlacementDevice".
        /// </summary>
        XPlacementDeviceMask = 0x0010,

        /// <summary>
        /// Corresponds to OT "YPlacementDevice".
        /// </summary>
        YPlacementDeviceMask = 0x0020,

        /// <summary>
        /// Corresponds to OT "XAdvanceDevice".
        /// </summary>
        XAdvanceDeviceMask  = 0x0040,

        /// <summary>
        /// Corresponds to OT "XAdvanceDevice".
        /// </summary>
        YAdvanceDeviceMask  = 0x0080,
    }

    /// <summary>
    /// Parser of GPOS table.
    /// </summary>
    public class GposParser : TransformationParserBase
    {
        /// <inheritdoc />
        protected override dynamic GetFontTable(GlyphTypeface typeface)
        {
            var openTypeTagsType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.OpenTypeTags");

            dynamic font = this.GetOpenTypeFont(typeface);

            dynamic gpos = Enum.ToObject(openTypeTagsType, 1196445523u);
            return new AccessPrivateWrapper(font.GetFontTable(gpos));
        }

        /// <inheritdoc />
        protected override dynamic GetFontTableHeader()
        {
            var headerType = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.GPOSHeader");

            return Activator.CreateInstance(headerType).AccessNonPublic();
        }

        /// <inheritdoc />
        protected override IGlyphTransformationTable GetSubstitutionTableBySubTableOffset(dynamic fontTable, int subTableOffset, int lookupTypeCode, LookupFlags lookupFlags)
        {
            switch ((PositioningLookupType)lookupTypeCode)
            {
                case PositioningLookupType.Single:
                    return this.ParseSinglePositioningTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.Pair:
                    return this.ParsePairPositioningTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.Cursive:
                    return this.ParseCursivePositioningTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.MarkToBase:
                    return this.ParseMarkToBasePositioningTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.MarkToLigature:
                    break; // Not supported
                case PositioningLookupType.MarkToMark:
                    return this.ParseMarkToMarkPositioningTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.Context:
                    return this.ParseContextSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.ChainingContext:
                    return this.ParseChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case PositioningLookupType.ExtensionPositioning:
                    return this.ParseExtensionLookupTable(subTableOffset, fontTable, lookupFlags);
                default:
                    throw new ArgumentOutOfRangeException("lookupTypeCode");
            }

            return null;
        }

        private IGlyphTransformationTable ParseSinglePositioningTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.SinglePositioningSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            ushort format = table.Format(fontTable.Wrapped);

            switch (format)
            {
                case 1:
                    return new ConstantPositioningTable
                    {
                        Coverage = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped))),
                        PositionChange = this.ParseValueRecord(fontTable, new AccessPrivateWrapper(table.Format1ValueRecord(fontTable.Wrapped))),
                        LookupFlags = lookupFlags
                    };
                case 2:
                    var coverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));
                    return new IndividualChangePositioningTable
                    {
                        Coverage = coverage,
                        PositionChanges =
                            coverage.CoveredGlyphIds.Keys
                                .Select(coverageIndex => (GlyphPositionChange)this.ParseValueRecord(fontTable, new AccessPrivateWrapper(table.Format2ValueRecord(fontTable.Wrapped, coverageIndex)))).ToList(),
                        LookupFlags = lookupFlags
                    };
                default:
                    throw new UnknownTableFormatException(type, format);
            }
        }

        private IGlyphTransformationTable ParsePairPositioningTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.PairPositioningSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            var coverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));
            var coveredGlyphCount = coverage.CoveredGlyphIds.Count();

            switch (format)
            {
                case 1:
                    var pairSetTables = this.GetEnumerableFromInternalList(
                        () => (ushort)coveredGlyphCount,
                        i => (dynamic)new AccessPrivateWrapper(table.Format1PairSet(fontTable.Wrapped, i))).ToList();

                    return new GlyphPairPositioningTable
                    {
                        Coverage = coverage,
                        PairSets =
                            pairSetTables.Select(pairSet => this.GetEnumerableFromInternalList(
                                () => (ushort)pairSet.PairValueCount(fontTable.Wrapped),
                                i => new PositioningPair
                                {
                                    SecondGlyphID = (ushort)pairSet.PairValueGlyph(fontTable.Wrapped, i),
                                    FirstGlyphPositionChange = this.ParseValueRecord(fontTable, new AccessPrivateWrapper(pairSet.FirstValueRecord(fontTable.Wrapped, i, table.FirstValueFormat(fontTable.Wrapped)))),
                                    SecondGlyphPositionChange = this.ParseValueRecord(fontTable, new AccessPrivateWrapper(pairSet.SecondValueRecord(fontTable.Wrapped, i, table.SecondValueFormat(fontTable.Wrapped)))),
                                }).ToList()).ToList(),
                        LookupFlags = lookupFlags
                    };
                case 2:
                    var class2Count = (ushort)table.Format2Class2Count(fontTable.Wrapped);

                    return new ClassPairPositioningTable
                    {
                        Coverage = coverage,
                        FirstClassDef = this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.Format2Class1Table(fontTable.Wrapped))),
                        SecondClassDef = this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.Format2Class2Table(fontTable.Wrapped))),
                        PairSets =
                            (from i in Enumerable.Range(0, (ushort)table.Format2Class1Count(fontTable.Wrapped))
                            select 
                                (from j in Enumerable.Range(0, class2Count)
                                select Tuple.Create(
                                    (GlyphPositionChange)this.ParseValueRecord(fontTable, new AccessPrivateWrapper(table.Format2FirstValueRecord(fontTable.Wrapped, class2Count, (ushort)i, (ushort)j))),
                                    (GlyphPositionChange)this.ParseValueRecord(fontTable, new AccessPrivateWrapper(table.Format2SecondValueRecord(fontTable.Wrapped, class2Count, (ushort)i, (ushort)j)))
                                )).ToList()).ToList(),
                        LookupFlags = lookupFlags
                    };
                default:
                    throw new UnknownTableFormatException(type, format);
            }
        }

        private IGlyphTransformationTable ParseCursivePositioningTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.CursivePositioningSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            var coverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));
            var coveredGlyphCount = coverage.CoveredGlyphIds.Count();

            return new CursivePositioningTable
            {
                Coverage = coverage,
                EntryExitRecords = 
                    this.GetEnumerableFromInternalList(
                        () => (ushort)coveredGlyphCount,
                        i => Tuple.Create(
                            (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(table.EntryAnchor(fontTable.Wrapped, i))),
                            (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(table.ExitAnchor(fontTable.Wrapped, i)))
                        )).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseMarkToBasePositioningTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.MarkToBasePositioningSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            dynamic markArrayTable = new AccessPrivateWrapper(table.Marks(fontTable.Wrapped));
            dynamic baseArrayTable = new AccessPrivateWrapper(table.Bases(fontTable.Wrapped));

            return new MarkToBasePositioningTable
            {
                MarkCoverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.MarkCoverage(fontTable.Wrapped))),
                BaseCoverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.BaseCoverage(fontTable.Wrapped))),
                MarkAnchorPoints = this.GetEnumerableFromInternalList(
                    () => fontTable.GetUShort(markArrayTable.offset),
                    i =>
                    new Tuple<ushort, AnchorPoint>(
                        (ushort)markArrayTable.Class(fontTable.Wrapped, i), 
                        (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(markArrayTable.MarkAnchor(fontTable.Wrapped, i)))
                    )).ToList(),
                BaseAnchorPoints = this.GetEnumerableFromInternalList(
                    () => fontTable.GetUShort(baseArrayTable.offset),
                    i => this.GetEnumerableFromInternalList(
                        () => table.ClassCount(fontTable.Wrapped),
                        j => (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(baseArrayTable.BaseAnchor(fontTable.Wrapped, i, table.ClassCount(fontTable.Wrapped), j)))
                    ).ToList()
                ).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseMarkToMarkPositioningTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.MarkToMarkPositioningSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            if (format != 1)
            {
                throw new UnknownTableFormatException(type, format);
            }

            dynamic mark1ArrayTable = new AccessPrivateWrapper(table.Mark1Array(fontTable.Wrapped));
            dynamic base2ArrayTable = new AccessPrivateWrapper(table.Marks2(fontTable.Wrapped));

            // MarkToMark is functionally identical to MarkToBase -> compiler can treat MTM as MTB 
            return new MarkToBasePositioningTable
            {
                MarkCoverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Mark1Coverage(fontTable.Wrapped))),
                BaseCoverage = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Mark2Coverage(fontTable.Wrapped))),
                MarkAnchorPoints = this.GetEnumerableFromInternalList(
                    () => fontTable.GetUShort(mark1ArrayTable.offset),
                    i =>
                    new Tuple<ushort, AnchorPoint>(
                        (ushort)mark1ArrayTable.Class(fontTable.Wrapped, i),
                        (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(mark1ArrayTable.MarkAnchor(fontTable.Wrapped, i)))
                    )).ToList(),
                BaseAnchorPoints = this.GetEnumerableFromInternalList(
                    () => fontTable.GetUShort(base2ArrayTable.offset),
                    i => this.GetEnumerableFromInternalList(
                        () => table.Mark1ClassCount(fontTable.Wrapped),
                        j => (AnchorPoint)this.ParseAnchor(fontTable, new AccessPrivateWrapper(base2ArrayTable.Anchor(fontTable.Wrapped, i, table.Mark1ClassCount(fontTable.Wrapped), j)))
                    ).ToList()
                ).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private GlyphPositionChange ParseValueRecord(dynamic fontTable, dynamic valueRecordTable)
        {
            var result = new GlyphPositionChange();
            int curentOffset = valueRecordTable.offset;

            var format = (ValueRecordFormatFlags)valueRecordTable.format;
            
            if ((format & ValueRecordFormatFlags.XPlacementFlag) != 0)
            {
                result.OffsetX = fontTable.GetShort(curentOffset);
                curentOffset += 2;
            }

            if ((format & ValueRecordFormatFlags.YPlacementFlag) != 0)
            {
                result.OffsetY = fontTable.GetShort(curentOffset);
                curentOffset += 2;
            }

            if ((format & ValueRecordFormatFlags.XAdvanceFlag) != 0)
            {
                result.AdvanceX = fontTable.GetShort(curentOffset);
                curentOffset += 2;
            }

            if ((format & ValueRecordFormatFlags.YAdvanceFlag) != 0)
            {
                result.AdvanceY = fontTable.GetShort(curentOffset);
            }

            return result;
        }

        private AnchorPoint ParseAnchor(dynamic fontTable, dynamic anchorTable)
        {
            return new AnchorPoint
            {
                X = anchorTable.XCoordinate(fontTable.Wrapped),
                Y = anchorTable.YCoordinate(fontTable.Wrapped)
            };
        }
    }
}
