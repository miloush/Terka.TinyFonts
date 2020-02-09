namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Compiler.Testing;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Positioning;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="PositioningCompiler"/> class.
    /// </summary>
    [TestFixture]
    public class PositioningCompilerTests
    {
        /// <summary>
        /// Tests that <see cref="PositioningCompiler.CompileTransformation" /> calls builder correctly on given transformaton.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="expected">The expected.</param>
        public void TestCompileTransformation(IGlyphTransformationTable table, IEnumerable<ITransition>[] expected)
        {
            var builder = new StateMachineBuilderStub();

            var compiler = new PositioningCompiler();
            compiler.CompileTransformation(table, builder);

            Assert.IsTrue(expected.ValuesEqual(builder.Paths, new PathEqualityComparer()));
        }
        
        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="ConstantPositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_ConstantPositioning_CallsBuilderCorrectly()
        {
            var table = new ConstantPositioningTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                }, 
                PositionChange = new GlyphPositionChange
                {
                    OffsetX = 2
                },
                LookupFlags = LookupFlags.IgnoreLigatures 
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures, 
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange
                                {
                                    OffsetX = 2
                                }
                            }, 
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures, 
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange
                                {
                                    OffsetX = 2
                                }
                            }, 
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="IndividualChangePositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_IndividualPositioning_CallsBuilderCorrectly()
        {
            var table = new IndividualChangePositioningTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                },
                PositionChanges = new[]
                {
                    new GlyphPositionChange
                    {
                        OffsetX = 3
                    },
                    new GlyphPositionChange
                    {
                        OffsetX = 4
                    }
                },
                LookupFlags = LookupFlags.IgnoreLigatures 
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures, 
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange
                                {
                                    OffsetX = 3
                                }
                            }, 
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures, 
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange
                                {
                                    OffsetX = 4
                                }
                            }, 
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="GlyphPairPositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_GlyphPairPositioning_CallsBuilderCorrectly()
        {
            /*
             * This test tests these pairs:
             * 1 2 => +X5 +X6
             * 1 3 => +X7 +X8
             * 4 2 => +X9 +X10
             * 4 3 => +X11 +X12
             */ 
            var table = new GlyphPairPositioningTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 4 }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                PairSets = new[]
                {
                    (IEnumerable<PositioningPair>)new[]
                    {
                        new PositioningPair
                        {
                            SecondGlyphID = 2,
                            FirstGlyphPositionChange = new GlyphPositionChange { OffsetX = 5 },
                            SecondGlyphPositionChange = new GlyphPositionChange { OffsetX = 6 },
                        },
                        new PositioningPair
                        {
                            SecondGlyphID = 3,
                            FirstGlyphPositionChange = new GlyphPositionChange { OffsetX = 7 },
                            SecondGlyphPositionChange = new GlyphPositionChange { OffsetX = 8 },
                        }
                    },
                    new[]
                    {
                        new PositioningPair
                        {
                            SecondGlyphID = 2,
                            FirstGlyphPositionChange = new GlyphPositionChange { OffsetX = 9 },
                            SecondGlyphPositionChange = new GlyphPositionChange { OffsetX = 10 },
                        },
                        new PositioningPair
                        {
                            SecondGlyphID = 3,
                            FirstGlyphPositionChange = new GlyphPositionChange { OffsetX = 11 },
                            SecondGlyphPositionChange = new GlyphPositionChange { OffsetX = 12 },
                        }
                    }
                }
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    },
                    new SimpleTransition {
                        GlyphId = 2,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 5 },
                                new GlyphPositionChange { OffsetX = 6 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    },
                    new SimpleTransition {
                        GlyphId = 3,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 7 },
                                new GlyphPositionChange { OffsetX = 8 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 4, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    },
                    new SimpleTransition {
                        GlyphId = 2,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 9 },
                                new GlyphPositionChange { OffsetX = 10 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 4, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    },
                    new SimpleTransition {
                        GlyphId = 3,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 11 },
                                new GlyphPositionChange { OffsetX = 12 }
                            },                             
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="ClassPairPositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_ClassPairPositioning_CallsBuilderCorrectly()
        {
            /*
             * This test tests these pairs:
             * (1 2) (5 6) => +X5 +X6
             * (1 2) (7 8) => +X7 +X8
             * (3 4) (5 6) => +X9 +X10
             * (3 4) (7 8) => +X11 +X12
             */
            var table = new ClassPairPositioningTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2, 3, 4 }
                },
                FirstClassDef = new ListGlyphClassDefinition
                {
                    FirstGlyphId = 1,
                    ClassIdList = new ushort[] { 0, 0, 1, 1 }                    
                },
                SecondClassDef = new ListGlyphClassDefinition
                {
                    FirstGlyphId = 5,
                    ClassIdList = new ushort[] { 0, 0, 1, 1 }
                },
                LookupFlags = LookupFlags.IgnoreLigatures,
                PairSets = new[]
                {
                    (IEnumerable<Tuple<GlyphPositionChange, GlyphPositionChange>>)new[]
                    {
                        new Tuple<GlyphPositionChange, GlyphPositionChange>(
                            new GlyphPositionChange { OffsetX = 5 },
                            new GlyphPositionChange { OffsetX = 6 }),
                        new Tuple<GlyphPositionChange, GlyphPositionChange>(
                            new GlyphPositionChange { OffsetX = 7 },
                            new GlyphPositionChange { OffsetX = 8 })
                    },
                    new[]
                    {
                        new Tuple<GlyphPositionChange, GlyphPositionChange>(
                            new GlyphPositionChange { OffsetX = 9 },
                            new GlyphPositionChange { OffsetX = 10 }),
                        new Tuple<GlyphPositionChange, GlyphPositionChange>(
                            new GlyphPositionChange { OffsetX = 11 },
                            new GlyphPositionChange { OffsetX = 12 })
                    }
                }
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SetTransition 
                    { 
                        GlyphIdSet = new HashSet<ushort> { 1, 2 }, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 5, 6 },
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 5 },
                                new GlyphPositionChange { OffsetX = 6 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SetTransition 
                    { 
                        GlyphIdSet = new HashSet<ushort> { 1, 2 }, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 7, 8 },
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 7 },
                                new GlyphPositionChange { OffsetX = 8 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SetTransition 
                    { 
                        GlyphIdSet = new HashSet<ushort> { 3, 4 }, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 5, 6 },
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 9 },
                                new GlyphPositionChange { OffsetX = 10 }
                            },                             
                        }
                    }
                },
                new[]
                {
                    new SetTransition 
                    { 
                        GlyphIdSet = new HashSet<ushort> { 3, 4 }, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 7, 8 },
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new PositioningAdjustmentAction
                        {
                            PositionChanges = new[]
                            {
                                new GlyphPositionChange { OffsetX = 11 },
                                new GlyphPositionChange { OffsetX = 12 }
                            },                             
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="CursivePositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_CursivePositioning_CallsBuilderCorrectly()
        {
            /*
             * This test tests these glyphs with these entry and exit anchor positions:
             * 1 - [1, 1] [2, 2]
             * 2 - [3, 3]   -
             * 3 -    -   [4, 4]
             * 
             */
            var table = new CursivePositioningTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2, 3 }
                },
                EntryExitRecords = new []
                {
                    Tuple.Create(new AnchorPoint { X = 1, Y = 1 }, new AnchorPoint { X = 2, Y = 2 }), 
                    Tuple.Create<AnchorPoint, AnchorPoint>(new AnchorPoint { X = 3, Y = 3 }, null), 
                    Tuple.Create<AnchorPoint, AnchorPoint>(null, new AnchorPoint { X = 4, Y = 4 })
                },
                LookupFlags = LookupFlags.IgnoreLigatures
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 1,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 2 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 1 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 2,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 2 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 3 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 3, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 1,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 4, Y = 4 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 1 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 3, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 2,
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 4, Y = 4 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 3 }
                        }
                    }
                }
            };

            this.TestCompileTransformation(table, expected);
        }

        /// <summary>
        /// Tests that CompileTransformation calls builder correctly when passed a <see cref="MarkToBasePositioningTable"/>.
        /// </summary>
        [Test]
        public void CompileTransformation_MarkToBasePositioning_CallsBuilderCorrectly()
        {
            /*
             * Bases: 1 2
             * Marks: 
             *  class 0: 3
             *  class 1: 4, 5
             */
            var table = new MarkToBasePositioningTable
            {
                BaseCoverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                },
                MarkCoverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 3, 4, 5 }
                },
                BaseAnchorPoints = new []
                {
                    new []
                    {                        
                        new AnchorPoint { X = 1, Y = 1 }, // For mark class 0
                        new AnchorPoint { X = 2, Y = 2 }, // For mark class 1
                    },
                    new []
                    {                        
                        new AnchorPoint { X = 3, Y = 3 }, // For mark class 0
                        new AnchorPoint { X = 4, Y = 4 }, // For mark class 1
                    },
                },                
                LookupFlags = LookupFlags.IgnoreLigatures,
                MarkAnchorPoints = new []
                {
                    Tuple.Create((ushort)0, new AnchorPoint { X = 5, Y = 5 }),
                    Tuple.Create((ushort)1, new AnchorPoint { X = 6, Y = 6 }),
                    Tuple.Create((ushort)1, new AnchorPoint { X = 7, Y = 7 })
                }
            };

            var expected = new[]
            {
                (IEnumerable<ITransition>)new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 3, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 1 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 5, Y = 5 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 4, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 2 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 6, Y = 6 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 5, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 2 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 7, Y = 7 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 3, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 3 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 5, Y = 5 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 4, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 4, Y = 4 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 6, Y = 6 }
                        }
                    }
                },
                new[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 2, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures
                    }, 
                    new SimpleTransition 
                    {
                        GlyphId = 5, 
                        HeadShift = 1, 
                        LookupFlags = LookupFlags.IgnoreLigatures,
                        Action = new AnchorPointToAnchorPointAction
                        {
                            PreviousGlyphAnchorPoint = new AnchorPoint { X = 4, Y = 4 },
                            CurrentGlyphAnchorPoint = new AnchorPoint { X = 7, Y = 7 }
                        }
                    }
                },
            };

            this.TestCompileTransformation(table, expected);
        }
    }
}
