namespace Terka.FontBuilder.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Media;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Context;
    using Terka.FontBuilder.Parser.Reflection;
    using Terka.FontBuilder.Parser.Reflection.Extensions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Tohle se zdokumentuje pozdeji.")]
    public abstract class TransformationParserBase : TableParserBase
    {
        /// <summary>
        /// Gets a collection of all scripts supported by the typeface. List of all known script tags can be found here: http://www.microsoft.com/typography/developers/opentype/scripttags.aspx .
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <returns>Collection of tags.</returns>
        public IEnumerable<Tag> GetScriptTags(GlyphTypeface typeface)
        {
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic fontTable = this.GetFontTable(typeface);

            if (!fontTable.IsPresent)
            {
                return Enumerable.Empty<Tag>();
            }

            dynamic scriptList = new AccessPrivateWrapper(fontTableHeader.GetScriptList(fontTable.Wrapped));

            var uintTags = this.GetEnumerableFromInternalList(
                () => scriptList.GetScriptCount(fontTable.Wrapped),
                p => (uint)scriptList.GetScriptTag(fontTable.Wrapped, p));

            return uintTags.Select(TagConverter.TagFromUint);
        }

        /// <summary>
        /// Gets a collection of all language systems supported by a single script the typeface. List of all known lang sys tags can be found here: http://www.microsoft.com/typography/developers/opentype/languagetags.aspx .
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="script">The script tag.</param>
        /// <returns>
        /// Collection of tags.
        /// </returns>
        public IEnumerable<Tag> GetLangSysTags(GlyphTypeface typeface, Tag script)
        {
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic fontTable = this.GetFontTable(typeface);

            if (!fontTable.IsPresent)
            {
                yield break;
            }

            dynamic scriptList = new AccessPrivateWrapper(fontTableHeader.GetScriptList(fontTable.Wrapped));
            dynamic scriptTable = new AccessPrivateWrapper(scriptList.FindScript(fontTable.Wrapped, TagConverter.UintFromTag(script)));

            if (scriptTable.IsNull)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            if (scriptTable.IsDefaultLangSysExists(fontTable.Wrapped))
            {
                yield return new Tag("dflt");
            }

            var uintTags = this.GetEnumerableFromInternalList(
                () => scriptTable.GetLangSysCount(fontTable.Wrapped),
                p => (uint)scriptTable.GetLangSysTag(fontTable.Wrapped, p));

            foreach (var uintTag in uintTags)
            {
                yield return TagConverter.TagFromUint(uintTag);
            }
        }

        /// <summary>
        /// Gets a collection of all optional features supported by a language system of a script. List of all known feature tags can be found here: http://www.microsoft.com/typography/otspec/featurelist.htm .
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="script">The script tag.</param>
        /// <param name="langSys">The language system tag.</param>
        /// <returns>
        /// Collection of tags.
        /// </returns>
        public IEnumerable<Tag> GetOptionalFeatureTags(GlyphTypeface typeface, Tag script, Tag langSys)
        {
            // Lang sys coverageTable only contains indices to the common feature list
            var optionalFeatureIndices = this.GetOptionalFeatureIndices(typeface, script, langSys);

            var allFeatureTags = this.GetAllFeatureTags(typeface);

            // Return features from the common list which have indices listed in the lang sys coverageTable
            return
                from tagWithIndex in allFeatureTags.Select((tag, index) => new { Tag = tag, Index = index })
                join optionalFeatureIndex in optionalFeatureIndices on tagWithIndex.Index equals optionalFeatureIndex
                select tagWithIndex.Tag;
        }

        /// <summary>
        /// Gets collection of all features supported by the typeface.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <returns>Collection of tags.</returns>
        public IEnumerable<Tag> GetAllFeatureTags(GlyphTypeface typeface)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic featureList = new AccessPrivateWrapper(fontTableHeader.GetFeatureList(fontTable.Wrapped));

            var uintTags = this.GetEnumerableFromInternalList(
                () => featureList.FeatureCount(fontTable.Wrapped),
                p => (uint)featureList.FeatureTag(fontTable.Wrapped, p));

            return uintTags.Select(TagConverter.TagFromUint);
        }

        /// <summary>
        /// Gets transformation tables for a required feature of given script and language system.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="script">The script tag.</param>
        /// <param name="langSys">The language system tag.</param>
        /// <returns>Collection of transformation tables.</returns>
        public IEnumerable<IGlyphTransformationTable> GetTransformationTablesForRequiredFeature(GlyphTypeface typeface, Tag script, Tag langSys)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            var lookupIndices = this.GetRequiredFeatureLookupIndices(typeface, script, langSys);
            return this.GetTransformationTablesByLookupIndices(fontTable, lookupIndices);
        }

        /// <summary>
        /// Gets transformation tables for given optional feature of given script and language system.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="script">The script atg.</param>
        /// <param name="langSys">The language system tag.</param>
        /// <param name="feature">The feature tag.</param>
        /// <returns>
        /// Collection of transformation tables.
        /// </returns>
        public IEnumerable<IGlyphTransformationTable> GetTransformationTablesForOptionalFeature(GlyphTypeface typeface, Tag script, Tag langSys, Tag feature)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            var lookupIndices = this.GetOptionalFeatureLookupIndices(typeface, script, langSys, feature);
            return this.GetTransformationTablesByLookupIndices(fontTable, lookupIndices);
        }

        /// <summary>
        /// Gets transformation table by a lookup index.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="lookupIndex">Lookup index.</param>
        /// <returns>Collection of transformation tables.</returns>
        public IEnumerable<IGlyphTransformationTable> GetTransformationTablesByLookupIndex(GlyphTypeface typeface, ushort lookupIndex)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            return this.GetTransformationTablesByLookupIndices(fontTable, new [] {lookupIndex});
        }

        /// <summary>
        /// Dumps information about the specified typeface into console output stream.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        public void Dump(GlyphTypeface typeface)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic featureList = new AccessPrivateWrapper(fontTableHeader.GetFeatureList(fontTable.Wrapped));
            dynamic lookupList = new AccessPrivateWrapper(fontTableHeader.GetLookupList(fontTable.Wrapped));

            for (ushort i = 0; i < featureList.FeatureCount(fontTable.Wrapped); i++)
            {
                Console.WriteLine(i + " " + TagConverter.TagFromUint((uint)featureList.FeatureTag(fontTable.Wrapped, i)).Label);

                dynamic featureTable = new AccessPrivateWrapper(featureList.FeatureTable(fontTable.Wrapped, i));
                for (ushort j = 0; j < featureTable.LookupCount(fontTable.Wrapped); j++)
                {
                    dynamic lookupTable = new AccessPrivateWrapper(lookupList.Lookup(fontTable.Wrapped, featureTable.LookupIndex(fontTable.Wrapped, j)));
                    var lookupFlags = (LookupFlags)lookupTable.LookupFlags();

                    var substitutionLookupType = (PositioningLookupType)lookupTable.LookupType();
                    Console.Write(
                        "\t" + j + " " + lookupTable.offset + " " + featureTable.LookupIndex(fontTable.Wrapped, j) + " " +
                            substitutionLookupType.ToString() + " ");

                    if (substitutionLookupType == PositioningLookupType.ExtensionPositioning)
                    {
                        var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ExtensionLookupTable");

                        var subTables = this.GetEnumerableFromInternalList(
                            () => lookupTable.SubTableCount(),
                            index => (dynamic)new AccessPrivateWrapper(type.Instantiate((ushort)lookupTable.SubtableOffset(fontTable.Wrapped, index))));

                        Console.Write(" [" + string.Join(", ", subTables.Select(subTable => ((PositioningLookupType)subTable.LookupType(fontTable.Wrapped)).ToString()).Distinct()) + "] ");
                    }

                    Console.Write("(");

                    var flagsStrings = new List<string>();
                    if ((lookupFlags & LookupFlags.RightToLeft) == LookupFlags.RightToLeft)
                    {
                        flagsStrings.Add("RTL");
                    }

                    if ((lookupFlags & LookupFlags.IgnoreBaseGlyphs) == LookupFlags.IgnoreBaseGlyphs)
                    {
                        flagsStrings.Add("IgnoreBaseGlyphs");
                    }

                    if ((lookupFlags & LookupFlags.IgnoreLigatures) == LookupFlags.IgnoreLigatures)
                    {
                        flagsStrings.Add("IgnoreLigatures");
                    }

                    if ((lookupFlags & LookupFlags.IgnoreMarks) == LookupFlags.IgnoreMarks)
                    {
                        flagsStrings.Add("IgnoreMarks");
                    }

                    if ((lookupFlags & LookupFlags.UseMarkFilteringSet) == LookupFlags.UseMarkFilteringSet)
                    {
                        flagsStrings.Add("UseMarkFilteringSet");
                    }

                    if ((lookupFlags & LookupFlags.MarkAttachmentTypeMask) > 0)
                    {
                        flagsStrings.Add("MarkAttachmentTypeMask=" + (lookupFlags & LookupFlags.MarkAttachmentTypeMask));
                    }

                    Console.WriteLine(string.Join(", ", flagsStrings) + ") x" + lookupTable.SubTableCount());
                }
            }
        }

        protected ICoverageTable ParseCoverageTable(dynamic fontTable, dynamic coverageTable)
        {
            ushort format = coverageTable.Format(fontTable.Wrapped);
            switch (format)
            {
                case 1:
                    return new ListCoverageTable
                    {
                        CoveredGlyphIdList = this.GetEnumerableFromInternalList(
                            () => coverageTable.Format1GlyphCount(fontTable.Wrapped),
                            i => (ushort)coverageTable.Format1Glyph(fontTable.Wrapped, i)).ToList()
                    };
                case 2:
                    return new RangeCoverageTable
                    {
                        CoverageRanges = this.GetEnumerableFromInternalList(
                            () => coverageTable.Format2RangeCount(fontTable.Wrapped),
                            i => new CoverageRange
                            {
                                MinCoveredId = (ushort)coverageTable.Format2RangeStartGlyph(fontTable.Wrapped, i),
                                MaxCoveredId = (ushort)coverageTable.Format2RangeEndGlyph(fontTable.Wrapped, i),
                                FirstCoveredCoverageIndex = (ushort)coverageTable.Format2RangeStartCoverageIndex(fontTable.Wrapped, i)
                            }).ToList()
                    };
                default:
                    throw new UnknownTableFormatException(coverageTable.Wrapped.GetType(), format);
            }
        }

        protected abstract IGlyphTransformationTable GetSubstitutionTableBySubTableOffset(dynamic fontTable, int subTableOffset, int lookupTypeCode, LookupFlags lookupFlags);

        protected IGlyphTransformationTable ParseContextSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ContextSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            switch (format)
            {
                case 1:
                    return this.ParseGlyphContextSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case 2:
                    return this.ParseClassContextSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case 3:
                    return this.ParseCoverageContextSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                default:
                    throw new UnknownTableFormatException(type, format);
            }
        }

        protected IGlyphTransformationTable ParseChainingSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ChainingSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));
            ushort format = table.Format(fontTable.Wrapped);

            switch (format)
            {
                case 1:
                    return this.ParseGlyphChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case 2:
                    return this.ParseClassChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                case 3:
                    return this.ParseCoverageChainingSubstitutionTable(subTableOffset, fontTable, lookupFlags);
                default:
                    throw new UnknownTableFormatException(type, format);
            }
        }

        protected IGlyphTransformationTable ParseExtensionLookupTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ExtensionLookupTable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var lookupType = (int)table.LookupType(fontTable.Wrapped);
            var lookupOffset = (int)table.LookupSubtableOffset(fontTable.Wrapped);

            return this.GetSubstitutionTableBySubTableOffset(fontTable, lookupOffset, lookupType, lookupFlags);
        }

        /// <summary>
        /// Gets the enumerable from WPF native parser list accessor functions.
        /// </summary>
        /// <typeparam name="T">Collection member type.</typeparam>
        /// <param name="countFunction">The count function.</param>
        /// <param name="getterFunction">The getter function.</param>
        /// <returns>Collection of values extracted from the list.</returns>
        protected IEnumerable<T> GetEnumerableFromInternalList<T>(Func<ushort> countFunction, Func<ushort, T> getterFunction)
        {
            for (ushort i = 0; i < countFunction(); i++)
            {
                yield return getterFunction(i);
            }
        }

        /// <summary>
        /// Gets the language system table.
        /// </summary>
        /// <param name="script">The script tag.</param>
        /// <param name="langSys">The language system tag.</param>
        /// <param name="fontTable">The font table.</param>
        /// <returns>Language system table</returns>
        private dynamic GetLangSysTable(Tag script, Tag langSys, dynamic fontTable)
        {
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic scriptList = new AccessPrivateWrapper(fontTableHeader.GetScriptList(fontTable.Wrapped));
            dynamic scriptTable = new AccessPrivateWrapper(scriptList.FindScript(fontTable.Wrapped, TagConverter.UintFromTag(script)));

            if (scriptTable.IsNull)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            dynamic langSysTable = langSys == null ? 
                new AccessPrivateWrapper(scriptTable.GetDefaultLangSysTable(fontTable.Wrapped)) : 
                new AccessPrivateWrapper(scriptTable.FindLangSys(fontTable.Wrapped, TagConverter.UintFromTag(langSys)));

            if (langSysTable.IsNull)
            {
                throw new ArgumentOutOfRangeException("langSys");
            }

            return langSysTable;
        }

        private IEnumerable<ushort> GetRequiredFeatureLookupIndices(GlyphTypeface typeface, Tag script, Tag langSys)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            dynamic langSysTable = this.GetLangSysTable(script, langSys, fontTable);

            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic featureList = fontTableHeader.GetFeatureList(fontTable.Wrapped);

            dynamic requiredFeatureTable = new AccessPrivateWrapper(langSysTable.RequiredFeature(fontTable.Wrapped, featureList));

            // The lang sys may not have a required feature
            if (requiredFeatureTable.IsNull)
            {
                return Enumerable.Empty<ushort>();
            }

            return this.GetLookupIndicesFromFeatureTable(typeface, requiredFeatureTable);
        }

        private IEnumerable<ushort> GetOptionalFeatureIndices(GlyphTypeface typeface, Tag script, Tag langSys)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            dynamic langSysTable = this.GetLangSysTable(script, langSys, fontTable);
            var optionalFeatureIndices = this.GetEnumerableFromInternalList(
                () => langSysTable.FeatureCount(fontTable.Wrapped), p => (ushort)langSysTable.GetFeatureIndex(fontTable.Wrapped, p));
            return optionalFeatureIndices;
        }

        private IEnumerable<ushort> GetLookupIndicesFromFeatureTable(GlyphTypeface typeface, dynamic featureTable)
        {
            dynamic fontTable = this.GetFontTable(typeface);

            return this.GetEnumerableFromInternalList(
                () => featureTable.LookupCount(fontTable.Wrapped),
                p => (ushort)featureTable.LookupIndex(fontTable.Wrapped, p));
        }

        private IEnumerable<ushort> GetOptionalFeatureLookupIndices(GlyphTypeface typeface, Tag script, Tag langSys, Tag feature)
        {
            dynamic fontTable = this.GetFontTable(typeface);
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic featureList = new AccessPrivateWrapper(fontTableHeader.GetFeatureList(fontTable.Wrapped));

            dynamic langSysTable = this.GetLangSysTable(script, langSys, fontTable);
            dynamic featureTable = new AccessPrivateWrapper(langSysTable.FindFeature(fontTable.Wrapped, featureList.Wrapped, TagConverter.UintFromTag(feature)));

            if (featureTable.IsNull)
            {
                throw new ArgumentOutOfRangeException("feature");
            }

            return this.GetLookupIndicesFromFeatureTable(typeface, featureTable);
        }

        public IEnumerable<IGlyphTransformationTable> GetTransformationTablesByLookupIndices(dynamic fontTable, IEnumerable<ushort> lookupIndices)
        {
            return lookupIndices.SelectMany<ushort, IGlyphTransformationTable>(i => this.GetTransformationTablesByLookupIndex(fontTable, i)).ToList();
        }

        private IEnumerable<IGlyphTransformationTable> GetTransformationTablesByLookupIndex(dynamic fontTable, ushort lookupIndex)
        {
            dynamic fontTableHeader = this.GetFontTableHeader();
            dynamic lookupList = new AccessPrivateWrapper(fontTableHeader.GetLookupList(fontTable.Wrapped));

            dynamic lookupTable = new AccessPrivateWrapper(lookupList.Lookup(fontTable.Wrapped, lookupIndex));

            return this.GetSubstitutionTablesByLookupTable(fontTable, lookupTable);
        }

        private IEnumerable<IGlyphTransformationTable> GetSubstitutionTablesByLookupTable(dynamic fontTable, dynamic lookupTable)
        {
            var lookupType = (int)lookupTable.LookupType();
            var subTableOffsets = this.GetEnumerableFromInternalList(
                () => lookupTable.SubTableCount(),
                i => (int)lookupTable.SubtableOffset(fontTable.Wrapped, i)).ToList();

            var lookupFlags = (LookupFlags)lookupTable.LookupFlags();

            return subTableOffsets.Select(offset => (IGlyphTransformationTable)this.GetSubstitutionTableBySubTableOffset(fontTable, offset, lookupType, lookupFlags)).ToList();
        }

        protected IGlyphClassDefinition ParseClassDef(dynamic fontTable, dynamic classDefTable)
        {
            var format = (ushort)classDefTable.Format(fontTable.Wrapped);
            switch (format)
            {
                case 1:
                    return new ListGlyphClassDefinition
                    {
                        FirstGlyphId = (ushort)classDefTable.Format1StartGlyph(fontTable.Wrapped),
                        ClassIdList = this.GetEnumerableFromInternalList(
                            () => classDefTable.Format1GlyphCount(fontTable.Wrapped),
                            i => (ushort)classDefTable.Format1ClassValue(fontTable.Wrapped, i))
                    };
                case 2:
                    return new RangeGlyphClassDefinition
                    {
                        ClassRanges = this.GetEnumerableFromInternalList(
                            () => classDefTable.Format2RangeCount(fontTable.Wrapped),
                            i => new
                            {
                                RangeStartGlyphId = (ushort)classDefTable.Format2RangeStartGlyph(fontTable.Wrapped, i),
                                RangeEndGlyphId = (ushort)classDefTable.Format2RangeEndGlyph(fontTable.Wrapped, i),
                                RangeClassValue = (ushort)classDefTable.Format2RangeClassValue(fontTable.Wrapped, i)
                            }).ToDictionary(p => new Tuple<ushort, ushort>(p.RangeStartGlyphId, p.RangeEndGlyphId), p => p.RangeClassValue)
                    };
                default:
                    throw new UnknownTableFormatException(classDefTable.Wrapped.GetType(), format);
            }
        }

        private ContextTransformationRule ParseContextTransformationRule(dynamic fontTable, dynamic rule, Func<dynamic, int, ushort> idProjection)
        {
            dynamic contextualLookupsTable = new AccessPrivateWrapper(rule.ContextualLookups(fontTable.Wrapped));

            return new ContextTransformationRule
            {
                Context = this.GetEnumerableFromInternalList(
                    () => rule.GlyphCount(fontTable.Wrapped),
                    i => (ushort)idProjection(rule, i)).ToList(),
                TransformationSets = this.GetEnumerableFromInternalList(
                    () => rule.SubstCount(fontTable.Wrapped),
                    i => new ContextTransformationSet
                    {
                        FirstGlyphIndex = contextualLookupsTable.SequenceIndex(fontTable.Wrapped, i),
                        Transformations = this.GetTransformationTablesByLookupIndex(fontTable, contextualLookupsTable.LookupIndex(fontTable.Wrapped, i))
                    }).ToList()
            };
        }

        private IGlyphTransformationTable ParseGlyphContextSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.GlyphContextSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var coverageTable = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));

            var ruleSetTables = this.GetEnumerableFromInternalList(
                () => coverageTable.CoveredGlyphIds.Count(),
                i => (dynamic)new AccessPrivateWrapper(table.RuleSet(fontTable.Wrapped, i))).ToList();

            return new GlyphContextTransformationTable
            {
                Coverage = coverageTable,
                TransformationRules = ruleSetTables
                    .Select(
                        ruleSetTable => this.GetEnumerableFromInternalList(
                            () => ruleSetTable.RuleCount(fontTable.Wrapped),
                            i => (ContextTransformationRule)this.ParseContextTransformationRule(
                                fontTable,
                                new AccessPrivateWrapper(table.Rule(fontTable.Wrapped, i)),
                                (Func<dynamic, int, ushort>)((ruleTable, index) => (ushort)ruleTable.GlyphId(fontTable.Wrapped, index)))).ToList()).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseClassContextSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ClassContextSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var coverageTable = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));

            var ruleSetTables = this.GetEnumerableFromInternalList(
                () => (ushort)table.ClassSetCount(fontTable.Wrapped),
                i => (dynamic)new AccessPrivateWrapper(table.ClassSet(fontTable.Wrapped, i))).ToList();

            return new ClassContextTransformationTable
            {
                Coverage = coverageTable,
                ClassDefinitions = this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.ClassDef(fontTable.Wrapped))),
                TransformationRules = ruleSetTables
                    .Select(
                        ruleSetTable => 
                            ruleSetTable.IsNull ?
                                Enumerable.Empty<ContextTransformationRule>() :
                                this.GetEnumerableFromInternalList(
                                    () => ruleSetTable.RuleCount(fontTable.Wrapped),
                                    i => (ContextTransformationRule)this.ParseContextTransformationRule(
                                        fontTable,
                                        new AccessPrivateWrapper(ruleSetTable.Rule(fontTable.Wrapped, i)),
                                        (Func<dynamic, int, ushort>)((ruleTable, index) => (ushort)ruleTable.ClassId(fontTable.Wrapped, index)))).ToList()).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseCoverageContextSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.CoverageContextSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var contextualLookupTable = (dynamic)new AccessPrivateWrapper(table.ContextualLookups(fontTable.Wrapped));
            return new CoverageContextTransformationTable
            {
                Coverages = this.GetEnumerableFromInternalList(
                    () => table.GlyphCount(fontTable.Wrapped),
                    i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.InputCoverage(fontTable.Wrapped, i)))).ToList(),
                TransformationSets = this.GetEnumerableFromInternalList(
                    () => table.SubstCount(fontTable.Wrapped),
                    i => new ContextTransformationSet
                    {
                        FirstGlyphIndex = contextualLookupTable.SequenceIndex(fontTable.Wrapped, i),
                        Transformations = this.GetTransformationTablesByLookupIndex(fontTable, contextualLookupTable.LookupIndex(fontTable.Wrapped, i))
                    }).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private ChainingContextTransformationRule ParseChainingTransformationRule(dynamic fontTable, dynamic rule, Func<dynamic, int, ushort> idProjection, ushort firstInputId)
        {
            const int GlyphIdSize = 2;

            // Load the lookback
            var lookBackCount = rule.GlyphCount(fontTable.Wrapped, rule.offset);
            int currentOffset = rule.offset + rule.sizeCount;

            var lookBackGlyphIds = this.GetEnumerableFromInternalList(
                () => lookBackCount,
                i => (ushort)idProjection(rule, (i * GlyphIdSize) + currentOffset)).ToList();

            currentOffset += lookBackCount * GlyphIdSize;

            // Load the context
            var contextCount = rule.GlyphCount(fontTable.Wrapped, currentOffset);
            currentOffset += rule.sizeCount;

            // The first element of the context comes from index of the rule set
            var contextGlyphIds = this.GetEnumerableFromInternalList(
                () => (ushort)(contextCount - 1),
                i => (ushort)idProjection(rule, (i * GlyphIdSize) + currentOffset)).Prepend(firstInputId).ToList();

            currentOffset += (contextCount - 1) * GlyphIdSize;

            // Load the context
            var lookAheadCount = rule.GlyphCount(fontTable.Wrapped, currentOffset);
            currentOffset += rule.sizeCount;

            var lookAheadGlyphIds = this.GetEnumerableFromInternalList(
                () => lookAheadCount,
                i => (ushort)idProjection(rule, (i * GlyphIdSize) + currentOffset)).ToList();

            currentOffset += lookAheadCount * GlyphIdSize;

            // Load the substitution lookup records
            var lookupSubstitutionRecordCount = fontTable.GetUShort(currentOffset);

            dynamic contextualLookupsTable = new AccessPrivateWrapper(rule.ContextualLookups(fontTable.Wrapped, currentOffset));

            return new ChainingContextTransformationRule
            {
                Lookback = lookBackGlyphIds,
                Context = contextGlyphIds,
                Lookahead = lookAheadGlyphIds,
                TransformationSets = this.GetEnumerableFromInternalList(
                    () => lookupSubstitutionRecordCount,
                    i => new ContextTransformationSet
                    {
                        FirstGlyphIndex = contextualLookupsTable.SequenceIndex(fontTable.Wrapped, i),
                        Transformations = this.GetTransformationTablesByLookupIndex(fontTable, contextualLookupsTable.LookupIndex(fontTable.Wrapped, i))
                    }).ToList()
            };
        }

        private IGlyphTransformationTable ParseGlyphChainingSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.GlyphChainingSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var coverageTable = (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));

            var ruleSetTables = this.GetEnumerableFromInternalList(
                () => (ushort)coverageTable.CoveredGlyphIds.Count(),
                i => (dynamic)new AccessPrivateWrapper(table.RuleSet(fontTable.Wrapped, i))).ToList();

            return new ChainingGlyphContextTransformationTable
            {
                Coverage = coverageTable,
                TransformationRules = ruleSetTables
                    .Select(
                        ruleSetTable => this.GetEnumerableFromInternalList(
                            () => ruleSetTable.RuleCount(fontTable.Wrapped),
                            i => (ChainingContextTransformationRule)this.ParseChainingTransformationRule(
                                fontTable,
                                new AccessPrivateWrapper(table.Rule(fontTable.Wrapped, i)),
                                (Func<dynamic, int, ushort>)((ruleTable, index) => (ushort)ruleTable.GlyphId(fontTable.Wrapped, index)),
                                coverageTable.CoveredGlyphIds[i])).ToList()).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseClassChainingSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.ClassChainingSubtable");
            dynamic table = new AccessPrivateWrapper(type.Instantiate(subTableOffset));

            var coverageTable = this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.Coverage(fontTable.Wrapped)));

            var ruleSetTables = this.GetEnumerableFromInternalList(
                () => (ushort)table.ClassSetCount(fontTable.Wrapped),
                i => (dynamic)new AccessPrivateWrapper(table.ClassSet(fontTable.Wrapped, i))).ToList();

            var LookbackClassDefinition = fontTable.GetUShort(table.offset +
                table.offsetBacktrackClassDef) == 0 ? RangeGlyphClassDefinition.CreateEmptyClassDef() : this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.BacktrackClassDef(fontTable.Wrapped)));
            var ContextClassDefinitions = this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.InputClassDef(fontTable.Wrapped)));
            var LookaheadClassDefinition = fontTable.GetUShort(table.offset +
                table.offsetLookaheadClassDef) == 0 ? RangeGlyphClassDefinition.CreateEmptyClassDef() : this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.LookaheadClassDef(fontTable.Wrapped)));

            return new ChainingClassContextTransformationTable
            {
                Coverage = coverageTable,
                LookbackClassDefinition = fontTable.GetUShort(table.offset + table.offsetBacktrackClassDef) == 0 ? 
                    RangeGlyphClassDefinition.CreateEmptyClassDef() : 
                    this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.BacktrackClassDef(fontTable.Wrapped))),
                ContextClassDefinitions = this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.InputClassDef(fontTable.Wrapped))),
                LookaheadClassDefinition = fontTable.GetUShort(table.offset + table.offsetLookaheadClassDef) == 0 ? 
                    RangeGlyphClassDefinition.CreateEmptyClassDef() : 
                    this.ParseClassDef(fontTable, new AccessPrivateWrapper(table.LookaheadClassDef(fontTable.Wrapped))),
                TransformationRules = ruleSetTables
                    .Select(
                        ruleSetTable => ruleSetTable.IsNull ?
                            Enumerable.Empty<ChainingContextTransformationRule>() :
                            this.GetEnumerableFromInternalList(
                                () => ruleSetTable.RuleCount(fontTable.Wrapped),
                                i => (ChainingContextTransformationRule)this.ParseChainingTransformationRule(
                                    fontTable,
                                    new AccessPrivateWrapper(ruleSetTable.Rule(fontTable.Wrapped, i)),
                                    (Func<dynamic, int, ushort>)((ruleTable, index) => (ushort)ruleTable.ClassId(fontTable.Wrapped, index)),
                                    (ushort)i)).ToList()).ToList(),
                LookupFlags = lookupFlags
            };
        }

        private IGlyphTransformationTable ParseCoverageChainingSubstitutionTable(int subTableOffset, dynamic fontTable, LookupFlags lookupFlags)
        {
            var type = typeof(GlyphTypeface).Assembly.GetType("MS.Internal.Shaping.CoverageChainingSubtable");
            dynamic table = new AccessPrivateWrapper(TypeExtensions.Instantiate(type, fontTable.Wrapped, subTableOffset));

            dynamic contextualLookupTable = new AccessPrivateWrapper(table.ContextualLookups(fontTable.Wrapped));

            return new ChainingCoverageContextSubstitutionTable
            {
                LookbackCoverages = this.GetEnumerableFromInternalList(
                    () => table.BacktrackGlyphCount(fontTable.Wrapped),
                    i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.BacktrackCoverage(fontTable.Wrapped, i)))).ToList(),
                ContextCoverages = this.GetEnumerableFromInternalList(
                    () => table.InputGlyphCount(fontTable.Wrapped),
                    i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.InputCoverage(fontTable.Wrapped, i)))).ToList(),
                LookaheadCoverages = this.GetEnumerableFromInternalList(
                    () => table.LookaheadGlyphCount(fontTable.Wrapped),
                    i => (ICoverageTable)this.ParseCoverageTable(fontTable, new AccessPrivateWrapper(table.LookaheadCoverage(fontTable.Wrapped, i)))).ToList(),
                TransformationSets = this.GetEnumerableFromInternalList(
                    () => contextualLookupTable.recordCount,
                    i => new ContextTransformationSet
                    {
                        FirstGlyphIndex = contextualLookupTable.SequenceIndex(fontTable.Wrapped, i),
                        Transformations = this.GetTransformationTablesByLookupIndex(fontTable, contextualLookupTable.LookupIndex(fontTable.Wrapped, i))
                    }).ToList(),
                LookupFlags = lookupFlags
            };
        }
    }
}