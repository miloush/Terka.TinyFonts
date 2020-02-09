namespace Terka.FontBuilder.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Substitution;

    /// <summary>
    /// Compiles substitution tables of an <see cref="GlyphTypeface"/> into <see cref="StateMachine"/>.
    /// </summary>
    public class SubstitutionCompiler : TransformationCompilerBase
    {
        /// <inheritdoc />
        public override void CompileTransformation(IGlyphTransformationTable transformation, IStateMachineBuilder builder)
        {
            if (transformation is SimpleReplacementSubstitutionTable)
            {
                var table = transformation as SimpleReplacementSubstitutionTable;

                var paths = table.Coverage.CoveredGlyphIds.Zip(
                    table.ReplacementGlyphIds, 
                    (coveragePair, replacementGlyphId) => new[]
                    {
                        new SimpleTransition {
                            GlyphId = coveragePair.Value,
                            HeadShift = 1, 
                            LookupFlags = table.LookupFlags,
                            Action = new SubstitutionAction
                            {
                                ReplacementGlyphIds = new[] { replacementGlyphId }, 
                                ReplacedGlyphCount = 1
                            }
                        }
                    });

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is DeltaSubstitutionTable)
            {
                var table = transformation as DeltaSubstitutionTable;

                var paths =
                    from coveragePair in table.Coverage.CoveredGlyphIds
                    select
                        new[]
                        {
                            new SimpleTransition 
                            {
                                GlyphId = coveragePair.Value, 
                                HeadShift = 1, 
                                LookupFlags = table.LookupFlags,
                                Action = new SubstitutionAction
                                {
                                    ReplacementGlyphIds = new[] { (ushort)(coveragePair.Value + table.GlyphIdDelta) }, 
                                    ReplacedGlyphCount = 1
                                }
                            }
                        };

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is LigatureSubstitutionTable)
            {
                var table = transformation as LigatureSubstitutionTable;

                var ligatureSetInfos = table.Coverage.CoveredGlyphIds.Zip(
                    table.LigatureSets, 
                    (coveragePair, ligatureSet) => new { CoveredGlyphId = coveragePair.Value, LigatureSet = ligatureSet });

                var paths =
                    from ligatureSetInfo in ligatureSetInfos
                    from ligature in ligatureSetInfo.LigatureSet
                    select this.ConvertLigatureToPath(ligatureSetInfo.CoveredGlyphId, ligature, table.LookupFlags).ToList();

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is MultipleSubstitutionTable)
            {
                var table = transformation as MultipleSubstitutionTable;

                var paths = table.Coverage.CoveredGlyphIds.Zip(
                    table.ReplacementSequences,
                    (coveragePair, replacementSequence) =>
                    {
                        var list = replacementSequence.ToList();
                        return new[]
                        {
                            new SimpleTransition
                            {
                                GlyphId = coveragePair.Value,
                                HeadShift = 1,
                                LookupFlags = table.LookupFlags,
                                Action = new SubstitutionAction
                                {                                    
                                    ReplacementGlyphIds = list,
                                    ReplacedGlyphCount = 1
                                }
                            }
                        };
                    });

                foreach (var path in paths)
                {
                    builder.AddPath(path);
                }
            }
            else if (transformation is ReverseChainingContextSubstitutionTable)
            {
                var table = transformation as ReverseChainingContextSubstitutionTable;

                builder.SetProcessingDirection(ProcessingDirection.EndToStart);

                // The entire machine is executed from the end of the string towards its start
                var lookbackCoveragesList = table.LookbackCoverages.ToList();
                var completeContext = (((IEnumerable<ICoverageTable>)lookbackCoveragesList)
                        .Reverse()
                        .Append(table.Coverage)
                        .Append(table.LookaheadCoverages)
                    ).Reverse().ToList();

                var contextCheckPathSeqment = completeContext
                    .Take(completeContext.Count - 1)
                    .Select(coverage => (ITransition)new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort>(coverage.CoveredGlyphIds.Select(p => p.Value)),
                        HeadShift = -1,
                        LookupFlags = table.LookupFlags
                    })
                    .Append(new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort>(completeContext.Last().CoveredGlyphIds.Select(p => p.Value)),
                        HeadShift = lookbackCoveragesList.Count,
                        LookupFlags = table.LookupFlags
                    }).ToList();

                int i = 0;
                var replacementGlyphIds = table.SubstituteGlyphIds.ToList();
                foreach (var coveredGlyphId in table.Coverage.CoveredGlyphIds.Values)
                {
                    var transition = new SimpleTransition
                    {
                        GlyphId = coveredGlyphId,
                        HeadShift = lookbackCoveragesList.Count,
                        LookupFlags = table.LookupFlags,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1,
                            ReplacementGlyphIds = new [] { replacementGlyphIds[i] }
                        }
                    };

                    builder.AddPath(contextCheckPathSeqment.Append(transition));

                    i++;
                }

                builder.AddPath(contextCheckPathSeqment.Append(new AlwaysTransition 
                {
                    HeadShift = lookbackCoveragesList.Count,
                    LookupFlags = table.LookupFlags
                }));
            }
            else
            {
                base.CompileTransformation(transformation, builder);
            }
        }

        /// <summary>
        /// Converts the ligature to path.
        /// </summary>
        /// <param name="leadingGlyphId">The leading glyph id.</param>
        /// <param name="ligature">The ligature.</param>
        /// <param name="lookupFlags">The lookup flags.</param>
        /// <returns>
        /// A path.
        /// </returns>
        private IEnumerable<ITransition> ConvertLigatureToPath(ushort leadingGlyphId, Ligature ligature, LookupFlags lookupFlags)
        {
            yield return new SimpleTransition {
                GlyphId = leadingGlyphId, 
                HeadShift = 1, 
                LookupFlags = lookupFlags 
            };

            var componentGlyphIdList = ligature.ComponentGlyphIds.ToList();

            // Some fonts contain "ligatures" 1 glyph long (those have leading glyph only).
            if (componentGlyphIdList.Any())
            {
                // Transitions to recognize the ligature.
                foreach (var componentGlyphId in componentGlyphIdList)
                {
                    yield return new SimpleTransition {
                        GlyphId = componentGlyphId, 
                        HeadShift = 1, 
                        LookupFlags = lookupFlags 
                    };
                }
            }

            yield return new AlwaysTransition
            {
                HeadShift = 0,
                LookupFlags = lookupFlags,
                Action = new SubstitutionAction
                {
                    ReplacedGlyphCount = (uint)componentGlyphIdList.Count + 1,
                    SkippedGlyphCount = 1,
                    ReplacementGlyphIds = new[] { ligature.LigatureGlyphId }
                }
            };
        }
    }
}