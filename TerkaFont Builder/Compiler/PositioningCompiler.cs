namespace Terka.FontBuilder.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;    
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Positioning;

    /// <summary>
    /// Compiles positioning tables of an <see cref="GlyphTypeface"/> into <see cref="StateMachine"/>.
    /// </summary>
    public class PositioningCompiler : TransformationCompilerBase
    {
        /// <inheritdoc />
        public override void CompileTransformation(IGlyphTransformationTable transformation, IStateMachineBuilder builder)
        {
            if (transformation is ConstantPositioningTable)
            {
                var table = transformation as ConstantPositioningTable;

                var paths = table.Coverage.CoveredGlyphIds.Values.Select(glyphId => new[]
                    {
                        new SimpleTransition {
                            GlyphId = glyphId,
                            HeadShift = 1,
                            LookupFlags = table.LookupFlags,
                            Action = new PositioningAdjustmentAction
                            {
                                PositionChanges = new[] { table.PositionChange }
                            }
                        }
                    });

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is IndividualChangePositioningTable)
            {
                var table = transformation as IndividualChangePositioningTable;

                var paths = table.Coverage.CoveredGlyphIds.Zip(
                    table.PositionChanges,
                    (coveragePair, positioningChange) => new[]
                    {
                        new SimpleTransition {
                            GlyphId = coveragePair.Value, 
                            HeadShift = 1, 
                            LookupFlags = table.LookupFlags,
                            Action = new PositioningAdjustmentAction
                            {

                                PositionChanges = new[] { positioningChange }, 
                            }
                        }
                    });

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is GlyphPairPositioningTable)
            {
                var table = transformation as GlyphPairPositioningTable;

                var coverageDictionary = table.Coverage.CoveredGlyphIds;

                var paths =
                    table.PairSets.SelectMany(
                        (pairSet, coverageIndex) => pairSet.Select(
                            pair => new[]
                            {
                                // First state just checks first glyph of the pair.
                                new SimpleTransition {
                                    GlyphId = coverageDictionary[(ushort)coverageIndex], 
                                    HeadShift = 1, 
                                    LookupFlags = table.LookupFlags
                                },

                                // Seconds state checks the second glyph and does the positioning.
                                new SimpleTransition {
                                    GlyphId =  pair.SecondGlyphID, 
                                    LookupFlags = table.LookupFlags,
                                    HeadShift = 0, // TODO: 0?
                                    Action = new PositioningAdjustmentAction
                                    {                                        
                                        PositionChanges = new[] { pair.FirstGlyphPositionChange, pair.SecondGlyphPositionChange },
                                    }
                                }
                            }));

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is ClassPairPositioningTable)
            {
                var table = transformation as ClassPairPositioningTable;

                var paths =
                    table
                        .PairSets
                        .Zip(table.FirstClassDef.ClassAssignments)
                        .SelectMany(
                            pairSetTuple => pairSetTuple
                                .Item1
                                .Zip(table.SecondClassDef.ClassAssignments)
                                .Select(pairTuple => new[]
                                {
                                    new SetTransition {
                                        GlyphIdSet = new HashSet<ushort>(pairSetTuple.Item2),
                                        HeadShift = 1, 
                                        LookupFlags = table.LookupFlags
                                    },
                                    new SetTransition 
                                    {
                                        GlyphIdSet = new HashSet<ushort>(pairTuple.Item2),
                                        HeadShift = 0,
                                        LookupFlags = table.LookupFlags,
                                        Action = new PositioningAdjustmentAction
                                        {
                                            PositionChanges = new[] { pairTuple.Item1.Item1, pairTuple.Item1.Item2 }
                                        }
                                    }
                                }));                

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is CursivePositioningTable)
            {
                var table = transformation as CursivePositioningTable;

                var glyphsWithEntryExits = table.Coverage.CoveredGlyphIds.Zip(table.EntryExitRecords, (coveragePair, entryExit) => new
                {
                    GlyphId = coveragePair.Value,
                    EntryAnchorPoint = entryExit.Item1,
                    ExitAnchorPoint = entryExit.Item2
                }).ToList();

                // Only pairs where the first glyph has exit anchor and the second glyph has entry anchor are valid
                var paths = 
                    from firstGlyph in glyphsWithEntryExits
                    where firstGlyph.ExitAnchorPoint != null
                    from secondGlyph in glyphsWithEntryExits
                    where secondGlyph.EntryAnchorPoint != null
                    select new []
                    {
                        // First state just checks first glyph of the pair.
                        new SimpleTransition {
                            GlyphId = firstGlyph.GlyphId, 
                            HeadShift = 1, 
                            LookupFlags = table.LookupFlags
                        },                        

                        // Seconds state checks the second glyph and does the positioning.
                        new SimpleTransition {
                            GlyphId = secondGlyph.GlyphId, 
                            LookupFlags = table.LookupFlags,
                            HeadShift = 0,
                            Action = new AnchorPointToAnchorPointAction
                            {                           
                                PreviousGlyphAnchorPoint = firstGlyph.ExitAnchorPoint,
                                CurrentGlyphAnchorPoint = secondGlyph.EntryAnchorPoint
                            }
                        }
                    };

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is MarkToBasePositioningTable)
            {
                var table = transformation as MarkToBasePositioningTable;

                /*var glyphIdWithMarkEntry = table.MarkCoverage.CoveredGlyphIds.Zip(
                    table.MarkAnchorPoints,
                    (coveragePair, markArrayEntry) => new KeyValuePair<ushort, Tuple<uushort, AnchorPoint>>(coveragePair.Value, markArrayEntry)).ToDictionary();

                var glyphIdWithBaseEntry = table.MarkCoverage.CoveredGlyphIds.Zip(
                    table.BaseAnchorPoints,
                    (coveragePair, baseArrayEntry) => new KeyValuePair<ushort, IEnumerable<AnchorPoint>>(coveragePair.Value, baseArrayEntry)).ToDictionary();
                */
                
                var glyphIdWithMarkEntry = table.MarkCoverage.CoveredGlyphIds.Zip(
                    table.MarkAnchorPoints,
                    (coveragePair, markArrayEntry) => new {
                        GlyphId = coveragePair.Value, 
                        ClassId = markArrayEntry.Item1,
                        AnchorPoint = markArrayEntry.Item2
                    });

                var glyphIdWithBaseEntry = table.BaseCoverage.CoveredGlyphIds.Zip(
                    table.BaseAnchorPoints,
                    (coveragePair, baseArrayEntry) => new 
                    {
                        GlyphId = coveragePair.Value, 
                        AnchorPointByClassId = baseArrayEntry.ToList()
                    });
 
                var paths = 
                    from baseGlyph in glyphIdWithBaseEntry
                    from markGlyph in glyphIdWithMarkEntry
                    select new []
                    {
                        new SimpleTransition
                        {
                            GlyphId = baseGlyph.GlyphId,
                            HeadShift = 1,
                            LookupFlags = table.LookupFlags
                        },
                        new SimpleTransition
                        {
                            GlyphId = markGlyph.GlyphId,
                            HeadShift = 1,
                            LookupFlags = table.LookupFlags,
                            Action = new AnchorPointToAnchorPointAction
                            {
                                PreviousGlyphAnchorPoint = baseGlyph.AnchorPointByClassId[markGlyph.ClassId],
                                CurrentGlyphAnchorPoint = markGlyph.AnchorPoint
                            }
                        }
                    };

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
        }
    }
}
