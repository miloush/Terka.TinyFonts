namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Compiler.Testing;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Context;
    using Terka.FontBuilder.Parser.Output.Substitution;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="TransformationCompilerBase"/> class.
    /// </summary>
    [TestFixture]
    public class TransformationCompilerBaseTests
    {
        /// <summary>
        /// Tests that <see cref="SubstitutionCompiler.CompileTransformation" /> calls builder correctly on given transformaton.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="expected">The expected.</param>
        public void TestCompileTransformation(IGlyphTransformationTable table, IEnumerable<ITransition>[] expected)
        {
            var builder = new StateMachineBuilderStub();

            var compiler = new SubstitutionCompiler();
            compiler.CompileTransformation(table, builder);

            Assert.IsTrue(expected.ValuesEqual(builder.Paths, new PathEqualityComparer()));
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_GlyphContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 2 in 1 2 3 4 for 5 and 3 in the same context for 6.
             * Replaces 8 in 7 8 9 10 for 11 and 9 in the same context for 12.  
             */

            var table = new GlyphContextTransformationTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 1, 7 }) },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationRules = new[]
                {
                    new[]
                    {
                        new ContextTransformationRule
                        {
                            Context = new ushort[] { 2, 3, 4 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 2, 3 }) },
                                             ReplacementGlyphIds = new ushort[] { 5, 6 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new ContextTransformationRule
                        {
                            Context = new ushort[] { 8, 9, 10 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 8, 9 }) },
                                             ReplacementGlyphIds = new ushort[] { 11, 12 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 5 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 6 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 8, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 10, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 8, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 11 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 8, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 10, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 9, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 12 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 8, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 10, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_ClassContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 102 in (101|201) (102|202) (103|203) (104|204) for 105 and 103 in the same context for 106.
             * Replaces 108 in (107|207) (108|208) (109|209) (110|210) for 111 and 109 in the same context for 112.  
             */

            var table = new ClassContextTransformationTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 101, 107 }) }, 
                ClassDefinitions = new RangeGlyphClassDefinition
                {
                    ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>
                    {
                        { new Tuple<ushort, ushort>(101, 101), 1 },
                        { new Tuple<ushort, ushort>(201, 201), 1 },
                        { new Tuple<ushort, ushort>(102, 102), 2 },
                        { new Tuple<ushort, ushort>(202, 202), 2 },
                        { new Tuple<ushort, ushort>(103, 103), 3 },
                        { new Tuple<ushort, ushort>(203, 203), 3 },
                        { new Tuple<ushort, ushort>(104, 104), 4 },
                        { new Tuple<ushort, ushort>(204, 204), 4 },
                        { new Tuple<ushort, ushort>(105, 105), 5 },
                        { new Tuple<ushort, ushort>(205, 205), 5 },
                        { new Tuple<ushort, ushort>(106, 106), 6 },
                        { new Tuple<ushort, ushort>(206, 206), 6 },
                        { new Tuple<ushort, ushort>(107, 107), 7 },
                        { new Tuple<ushort, ushort>(207, 207), 7 },
                        { new Tuple<ushort, ushort>(108, 108), 8 },
                        { new Tuple<ushort, ushort>(208, 208), 8 },
                        { new Tuple<ushort, ushort>(109, 109), 9 },
                        { new Tuple<ushort, ushort>(209, 209), 9 },
                        { new Tuple<ushort, ushort>(110, 110), 10 },
                        { new Tuple<ushort, ushort>(210, 210), 10 }
                    }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationRules = new[]
                {
                    new[]
                    {
                        new ContextTransformationRule
                        {
                            Context = new ushort[] { 2, 3, 4 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 102, 103 }) },
                                             ReplacementGlyphIds = new ushort[] { 105, 106 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new ContextTransformationRule
                        {
                            Context = new ushort[] { 8, 9, 10 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 108, 109 }) },
                                             ReplacementGlyphIds = new ushort[] { 111, 112 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 102, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 105 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 103, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 106 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 108, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 111 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 109, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 112 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_CoverageContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 102 in (101|201) (102|202) (103|203) (104|204) for 105 and 103 in the same context for 106.
             */

            var table = new CoverageContextTransformationTable
            {
                Coverages = new ICoverageTable[]
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 101, 201 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 102, 202 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 103, 203 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 104, 204 }) }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationSets = new[] {
                    new ContextTransformationSet
                    {
                        FirstGlyphIndex = 1,
                        Transformations = new[]
                        {
                            new SimpleReplacementSubstitutionTable
                            {
                                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 102, 103 }) },
                                ReplacementGlyphIds = new ushort[] { 105, 106 }, 
                                LookupFlags = LookupFlags.IgnoreMarks
                            }
                        }
                    }   
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 102, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 105 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 103, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 106 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -4, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_ChainingGlyphContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 5 in 1 2 3 | 4 5 6 | 7 5 9 for 105 and 6 in the same context for 106.
             * Replaces 14 in 10 11 12 | 13 14 15 | 16 17 18 for 114 and 15 in the same context for 115.  
             */

            var table = new ChainingGlyphContextTransformationTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 4, 13 }) },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationRules = new[]
                {
                    new[]
                    {
                        new ChainingContextTransformationRule
                        {
                            Lookback = new ushort[] { 3, 2, 1 },
                            Context = new ushort[] { 5, 6 },
                            Lookahead = new ushort[] { 7, 5, 9 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 5, 6 }) },
                                             ReplacementGlyphIds = new ushort[] { 105, 106 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new ChainingContextTransformationRule
                        {
                            Lookback = new ushort[] { 12, 11, 10 },
                            Context = new ushort[] { 14, 15 },
                            Lookahead = new ushort[] { 16, 17, 18 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 14, 15 }) },
                                             ReplacementGlyphIds = new ushort[] { 114, 115 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 105 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 106 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 9, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                },
                // Second rule
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 10, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 11, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 12, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 13, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 14, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 15, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 16, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 17, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 18, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 14, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 114 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 10, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 11, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 12, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 13, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 14, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 15, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 16, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 17, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 18, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 15, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 115 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 10, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 11, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 12, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 13, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 14, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 15, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 16, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 17, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 18, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_ChainingClassContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 105 in (101|201) (102|202) (103|203) | (104|204) (105|205) (106|206) | (107|207) (108|208) (109|209) for 305 and 106 in the same context for 306.
             * Replaces 105 in (110|210) (111|211) (112|212) | (113|213) (114|214) (115|215) | (116|216) (117|217) (118|218) for 305 and 106 in the same context for 306.
             */

            var table = new ChainingClassContextTransformationTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 104, 113 }) },
                LookupFlags = LookupFlags.IgnoreLigatures,
                ContextClassDefinitions = new RangeGlyphClassDefinition
                {
                    ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>
                    {
                        { new Tuple<ushort, ushort>(104, 104), 1 },
                        { new Tuple<ushort, ushort>(204, 204), 1 },
                        { new Tuple<ushort, ushort>(105, 105), 2 },
                        { new Tuple<ushort, ushort>(205, 205), 2 },
                        { new Tuple<ushort, ushort>(106, 106), 3 },
                        { new Tuple<ushort, ushort>(206, 206), 3 },
                        { new Tuple<ushort, ushort>(113, 113), 4 },
                        { new Tuple<ushort, ushort>(213, 213), 4 },
                        { new Tuple<ushort, ushort>(114, 114), 5 },
                        { new Tuple<ushort, ushort>(214, 214), 5 },
                        { new Tuple<ushort, ushort>(115, 115), 6 },
                        { new Tuple<ushort, ushort>(215, 215), 6 },
                    }
                },
                LookbackClassDefinition = new RangeGlyphClassDefinition
                {
                    ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>
                    {
                        { new Tuple<ushort, ushort>(101, 101), 7 },
                        { new Tuple<ushort, ushort>(201, 201), 7 },
                        { new Tuple<ushort, ushort>(102, 102), 8 },
                        { new Tuple<ushort, ushort>(202, 202), 8 },
                        { new Tuple<ushort, ushort>(103, 103), 9 },
                        { new Tuple<ushort, ushort>(203, 203), 9 },
                        { new Tuple<ushort, ushort>(110, 110), 10 },
                        { new Tuple<ushort, ushort>(210, 210), 10 },
                        { new Tuple<ushort, ushort>(111, 111), 11 },
                        { new Tuple<ushort, ushort>(211, 211), 11 },
                        { new Tuple<ushort, ushort>(112, 112), 12 },
                        { new Tuple<ushort, ushort>(212, 212), 12 },
                    }
                },
                LookaheadClassDefinition = new RangeGlyphClassDefinition
                {
                    ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>
                    {
                        { new Tuple<ushort, ushort>(107, 107), 13 },
                        { new Tuple<ushort, ushort>(207, 207), 13 },
                        { new Tuple<ushort, ushort>(108, 108), 14 },
                        { new Tuple<ushort, ushort>(208, 208), 14 },
                        { new Tuple<ushort, ushort>(109, 109), 15 },
                        { new Tuple<ushort, ushort>(209, 209), 15 },
                        { new Tuple<ushort, ushort>(116, 116), 16 },
                        { new Tuple<ushort, ushort>(216, 216), 16 },
                        { new Tuple<ushort, ushort>(117, 117), 17 },
                        { new Tuple<ushort, ushort>(217, 217), 17 },
                        { new Tuple<ushort, ushort>(118, 118), 18 },
                        { new Tuple<ushort, ushort>(218, 218), 18 }
                    }
                },
                TransformationRules = new[]
                {
                    new[]
                    {
                        new ChainingContextTransformationRule
                        {
                            Lookback = new ushort[] { 9, 8, 7 },
                            Context = new ushort[] { 2, 3 },
                            Lookahead = new ushort[] { 13, 14, 15 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 105, 106 }) },
                                             ReplacementGlyphIds = new ushort[] { 305, 306 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new ChainingContextTransformationRule
                        {
                            Lookback = new ushort[] { 12, 11, 10 },
                            Context = new ushort[] { 5, 6 },
                            Lookahead = new ushort[] { 16, 17, 18 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                         new SimpleReplacementSubstitutionTable
                                         {
                                             Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 114, 115 }) },
                                             ReplacementGlyphIds = new ushort[] { 314, 315 }, 
                                             LookupFlags = LookupFlags.IgnoreMarks
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 105, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 305 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 106, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 306 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                },
                // Second rule
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 111, 211 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 112, 212 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 113, 213 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 114, 214 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 115, 215 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 116, 216 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 117, 217 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 118, 218 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 114, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 314 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 111, 211 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 112, 212 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 113, 213 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 114, 214 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 115, 215 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 116, 216 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 117, 217 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 118, 218 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 115, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 315 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 110, 210 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 111, 211 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 112, 212 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 113, 213 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 114, 214 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 115, 215 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 116, 216 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 117, 217 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 118, 218 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_ChainingCoverageContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 104 in (101|201) (102|202) (103|203) | (104|204) (105|205) (106|206) | (107|207) (108|208) (109|209) for 304 and 105 in the same context for 305.
             */

            var table = new ChainingCoverageContextSubstitutionTable
            {
                ContextCoverages = new ICoverageTable[]
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 104, 204 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 105, 205 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 106, 206 }) }
                },
                LookbackCoverages = new ICoverageTable[]
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 103, 203 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 102, 202 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 101, 201 }) }
                },
                LookaheadCoverages = new ICoverageTable[]
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 107, 207 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 108, 208 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 109, 209 }) }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationSets = new[] {
                    new ContextTransformationSet
                    {
                        FirstGlyphIndex = 1,
                        Transformations = new[]
                        {
                            new SimpleReplacementSubstitutionTable
                            {
                                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 104, 105 }) },
                                ReplacementGlyphIds = new ushort[] { 304, 305 }, 
                                LookupFlags = LookupFlags.IgnoreMarks
                            }
                        }
                    }   
                }
            };

            var subMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 104, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 304 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 105, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 305 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 108, 208 }, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 109, 209 }, HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = -3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when given two nested context transformations.
        /// </summary>
        [Test]
        public void CompileTransformation_NestedGlyphContextTransformation_CallsBuilderCorrectly()
        {
            /* Replaces 3 in context 2 3 4 in context 1 2 3 4 5 for 6.
             */

            var table = new GlyphContextTransformationTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 1 }) },
                LookupFlags = LookupFlags.IgnoreLigatures,
                TransformationRules = new[]
                {
                    new[]
                    {
                        new ContextTransformationRule
                        {
                            Context = new ushort[] { 2, 3, 4, 5 },
                            TransformationSets = new[]
                            {
                                new ContextTransformationSet
                                {
                                    FirstGlyphIndex = 1,
                                    Transformations = new[]
                                    {
                                        new GlyphContextTransformationTable
                                        {
                                            Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 2 }) },
                                            LookupFlags = LookupFlags.IgnoreMarks,
                                            TransformationRules = new[]
                                            {
                                                new[]
                                                {
                                                    new ContextTransformationRule
                                                    {
                                                        Context = new ushort[] { 3, 4 },
                                                        TransformationSets = new[]
                                                        {
                                                            new ContextTransformationSet
                                                            {
                                                                FirstGlyphIndex = 1,
                                                                Transformations = new[]
                                                                {
                                                                     new SimpleReplacementSubstitutionTable
                                                                     {
                                                                         Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 3 }) },
                                                                         ReplacementGlyphIds = new ushort[] { 6 },
                                                                         LookupFlags = LookupFlags.IgnoreBaseGlyphs
                                                                     }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var subMachineEntryState = new State();
            var subSubMachineEntryState = new State();
            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = -5, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new SimpleTransition { GlyphId = 4, HeadShift = -3, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subSubMachineEntryState },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreBaseGlyphs, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 6 } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subSubMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = -5, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreMarks, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new SimpleTransition { GlyphId = 4, HeadShift = -3, LookupFlags = LookupFlags.IgnoreMarks, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subSubMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1 }, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = subMachineEntryState } } } }
                },
                new[]
                {
                    (ITransition)new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures } ,
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 5, HeadShift = -5, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 0, ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId } } },
                    new AlwaysTransition { HeadShift = 0, TargetState = subMachineEntryState },
                    new SimpleTransition { GlyphId = SubMachineBuilder.ContextTerminatorGlyphId, HeadShift = 0, Action = new SubstitutionAction { ReplacedGlyphCount = 1} }
                }
            };

            this.TestCompileTransformation(table, expected);
        }
    }
}
