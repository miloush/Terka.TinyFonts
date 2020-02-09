namespace Terka.FontBuilder.Compiler
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Compiler.Testing;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Substitution;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="SubstitutionCompiler"/> class.
    /// </summary>
    [TestFixture]
    public class SubstitutionCompilerTests
    {
        /// <summary>
        /// Tests that <see cref="SubstitutionCompiler.CompileTransformation" /> calls builder correctly on given transformaton.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="expectedProcessingDirection">The expected processing direction.</param>
        public void TestCompileTransformation(IGlyphTransformationTable table, IEnumerable<ITransition>[] expected, ProcessingDirection expectedProcessingDirection = ProcessingDirection.StartToEnd)
        {
            var builder = new StateMachineBuilderStub();

            var compiler = new SubstitutionCompiler();
            compiler.CompileTransformation(table, builder);

            Assert.IsTrue(expected.ValuesEqual(builder.Paths, new PathEqualityComparer()));
            Assert.AreEqual(expectedProcessingDirection, builder.ProcessingDirection);
        }

        /// <summary>
        /// Tests that CompileTransformation calls the builder correctly when passed a <see cref="SimpleReplacementSubstitutionTable" />.
        /// </summary>
        [Test]
        public void CompileTransformation_SimpleSubstitution_CallsBuilderCorrectly()
        {
            var table = new SimpleReplacementSubstitutionTable
            {
                Coverage = new ListCoverageTable               
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                }, 
                ReplacementGlyphIds = new ushort[] { 3, 4 },
                LookupFlags = LookupFlags.IgnoreLigatures
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 3 },
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition {
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 4 },
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls the builder correctly when passed a <see cref="DeltaSubstitutionTable" />.
        /// </summary>
        [Test]
        public void CompileTransformation_DeltaSubstitution_CallsBuilderCorrectly()
        {
            var table = new DeltaSubstitutionTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                },
                GlyphIdDelta = 10,
                LookupFlags = LookupFlags.IgnoreLigatures
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 11 },
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition {
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 12 },
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls the builder correctly when passed a <see cref="SimpleReplacementSubstitutionTable" />.
        /// </summary>
        [Test]
        public void CompileTransformation_LigatureSubstitution_CallsBuilderCorrectly()
        {
            /* Following ligatures are used in this test:
             * 1 2 3 -> 101
             * 1 2 4 -> 102
             * 5 6 7 -> 201
             * 5 6 8 -> 202
             */
            var table = new LigatureSubstitutionTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 5, 10 }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                LigatureSets = new[]
                {
                    new[]
                    {
                        new Ligature
                        {
                            ComponentGlyphIds = new ushort[] { 2, 3 }, 
                            LigatureGlyphId = 101
                        }, 
                        new Ligature
                        {
                            ComponentGlyphIds = new ushort[] { 2, 4 }, 
                            LigatureGlyphId = 102
                        }
                    }, 
                    new[]
                    {
                        new Ligature
                        {
                            ComponentGlyphIds = new ushort[] { 6, 7 }, 
                            LigatureGlyphId = 201
                        }, 
                        new Ligature
                        {
                            ComponentGlyphIds = new ushort[] { 6, 8 }, 
                            LigatureGlyphId = 202
                        }
                    },
                    new[]
                    {
                        new Ligature
                        {
                            ComponentGlyphIds = new ushort[] { }, 
                            LigatureGlyphId = 301
                        }
                    }
                }
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new SimpleTransition { GlyphId = 3, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new AlwaysTransition { HeadShift = 0, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 3, SkippedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 101 } } }
                }, 
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new SimpleTransition { GlyphId = 2, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new SimpleTransition { GlyphId = 4, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures }, 
                    new AlwaysTransition { HeadShift = 0, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 3, SkippedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 102 } } }
                },                                                                                                                                                                                        
                new ITransition[]                                                                                                                                                                         
                {                                                                                                                                                                                         
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new SimpleTransition { GlyphId = 7, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new AlwaysTransition { HeadShift = 0, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 3, SkippedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 201 } } }
                },                                                                                                                                                                                        
                new ITransition[]                                                                                                                                                                         
                {                                                                                                                                                                                         
                    new SimpleTransition { GlyphId = 5, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new SimpleTransition { GlyphId = 6, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new SimpleTransition { GlyphId = 8, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                       
                    new AlwaysTransition { HeadShift = 0, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 3, SkippedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 202 } } }
                },                                                                                                                                                                                        
                new ITransition[]                                                                                                                                                                         
                {                                                                                                                                                                                         
                    new SimpleTransition { GlyphId = 10, HeadShift = 1, LookupFlags = LookupFlags.IgnoreLigatures },                                                                                     
                    new AlwaysTransition { HeadShift = 0, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 1, SkippedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 301 } } }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls the builder correctly when passed a <see cref="MultipleSubstitutionTable" />.
        /// </summary>
        [Test]
        public void CompileTransformation_MultipleSubstitution_CallsBuilderCorrectly()
        {
            /**
             * Following replacements are used in this test:
             * 1 -> 3 4 5
             * 2 -> 6 7 8
             */
            var table = new MultipleSubstitutionTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                }, 
                ReplacementSequences = new[]
                {
                    new ushort[] { 3, 4, 5 }, 
                    new ushort[] { 6, 7, 8 }
                },
                LookupFlags = LookupFlags.IgnoreLigatures 
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 3, 4, 5 },
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition {
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 6, 7, 8 },
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly.
        /// </summary>
        [Test]
        public void CompileTransformation_ReverseChainingContextSubsitutionTable_CallsBuilderCorrectly()
        {
            /* Replaces 104 in (101|201) (102|202) (103|203) (104|204) (105|205) (106|206) (107|208) for 304 and 204 in the same context for 404.
             */

            var table = new ReverseChainingContextSubstitutionTable
            {
                Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 104, 204 }) },
                LookbackCoverages = new []
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 103, 203 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 102, 202 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 101, 201 }) }
                },
                LookaheadCoverages = new[]
                {
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 105, 205 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 106, 206 }) },
                    new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 107, 207 }) }
                },
                SubstituteGlyphIds = new ushort[] { 304, 404 },
                LookupFlags = LookupFlags.IgnoreLigatures
            };

            var expected = new IEnumerable<ITransition>[]
            {
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 104, HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 304 } } }
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SimpleTransition { GlyphId = 204, HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures, Action = new SubstitutionAction { ReplacedGlyphCount = 1, ReplacementGlyphIds = new ushort[] { 404 } } } 
                },
                new[]
                {
                    (ITransition)new SetTransition { GlyphIdSet = new HashSet<ushort>{ 107, 207 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 106, 206 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 105, 205 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 104, 204 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 103, 203 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 102, 202 }, HeadShift = -1, LookupFlags = LookupFlags.IgnoreLigatures },
                    new SetTransition { GlyphIdSet = new HashSet<ushort>{ 101, 201 }, HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures },
                    new AlwaysTransition { HeadShift = 3, LookupFlags = LookupFlags.IgnoreLigatures }
                }
            };

            this.TestCompileTransformation(table, expected, ProcessingDirection.EndToStart);
        }
    }
}