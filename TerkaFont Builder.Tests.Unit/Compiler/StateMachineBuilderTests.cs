namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using NUnit.Framework;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Compiler.Output.Testing;
    using Terka.FontBuilder.Compiler.Testing;
    using Terka.FontBuilder.Extensions;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="StateMachineBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class StateMachineBuilderTests
    {
        /// <summary>
        /// Tests that multiple calls to AddPath yield correct state machine.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="expectedEntryState">Expected state of the entry.</param>
        public void TestAddPaths(ITransition[][] paths, State expectedEntryState)
        {
            var builder = new StateMachineBuilder();

            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var result = builder.GetStateMachine();

            var expectedMachine = new StateMachine(expectedEntryState);

            Assert.IsTrue(new StateMachineEqualityComparer().Equals(expectedMachine, result));
        }

        /// <summary>
        /// Tests that two calls to AddPath with common prefix result in the second path being ignored.
        /// </summary>
        [Test]
        public void AddPath_ForkingPathWithAmbiguousCommonState_OnlyUsesFirstAddedPath()
        {
            var paths = new ITransition[][]
            {
                new[]
                {
                    new SimpleTransition
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    }, 
                    new SimpleTransition
                    {
                        GlyphId = 2, 
                        HeadShift = 2, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 2, 
                            ReplacementGlyphIds = new ushort[] { 2 }
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition
                    {
                        GlyphId = 1, 
                        HeadShift = 2, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    }, 
                    new SimpleTransition
                    {
                        GlyphId = 3, 
                        HeadShift = 3, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 3, 
                            ReplacementGlyphIds = new ushort[] { 3 }
                        }
                    }
                }
            };

            var expectedEntryState = new State
            {                
                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }, 

                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition {
                                    GlyphId = 2, 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {

                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                }
                            }                            
                        }
                    }
                }
            };

            this.TestAddPaths(paths, expectedEntryState);
        }

        /// <summary>
        /// Tests that two calls to AddPath with common prefix result in a correct state machine.
        /// </summary>
        [Test]
        public void AddPath_ForkingPath_BuildsCorrectMachine()
        {
            var paths = new ITransition[][]
            {
                new[]
                {
                    new SimpleTransition {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    }, 
                    new SimpleTransition {
                        GlyphId = 2, 
                        HeadShift = 2, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 2, 
                            ReplacementGlyphIds = new ushort[] { 2 }
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    },
                    new SimpleTransition {
                        GlyphId = 3, 
                        HeadShift = 3, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 3, 
                            ReplacementGlyphIds = new ushort[] { 3 }
                        }
                    }
                }
            };

            var expectedEntryState = new State
            {
                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 },                             
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition {
                                    GlyphId = 2, 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition {
                                    GlyphId = 3, 
                                    HeadShift = 3, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 3, 
                                        ReplacementGlyphIds = new ushort[] { 3 }
                                    },
                                    TargetState = new State()
                                }
                            }
                        }
                    }
                }
            };

            this.TestAddPaths(paths, expectedEntryState);
        }

        /// <summary>
        /// Tests that two calls to AddPath with set transitions with common prefix result in a correct state machine where the transitions
        /// are ungrouped.
        /// </summary>
        [Test]
        public void AddPath_ForkingSetPath_BuildsCorrectMachine()
        {
            var paths = new ITransition[][]
            {
                new[]
                {
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 1, 2 }, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    },
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 3, 4 }, 
                        HeadShift = 2, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 2, 
                            ReplacementGlyphIds = new ushort[] { 2 }
                        }
                    }
                }, 
                new[]
                {
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 1, 2 }, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    },
                    new SetTransition 
                    {
                        GlyphIdSet = new HashSet<ushort> { 5, 6 }, 
                        HeadShift = 3, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 3, 
                            ReplacementGlyphIds = new ushort[] { 3 }
                        }
                    }
                }
            };

            var expectedEntryState = new State
            {
                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1,
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 } 
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition 
                                {
                                    GlyphId = 3,
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 4, 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 5,
                                    HeadShift = 3, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 3, 
                                        ReplacementGlyphIds = new ushort[] { 3 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 6,
                                    HeadShift = 3, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 3, 
                                        ReplacementGlyphIds = new ushort[] { 3 }
                                    },
                                    TargetState = new State()
                                },

                            }
                        }
                    },
                    new SimpleTransition 
                    {
                        GlyphId = 2,
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 } 
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition 
                                {
                                    GlyphId = 3, // 4 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 4, 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 5,
                                    HeadShift = 3, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 3, 
                                        ReplacementGlyphIds = new ushort[] { 3 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 6,
                                    HeadShift = 3, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 3, 
                                        ReplacementGlyphIds = new ushort[] { 3 }
                                    },
                                    TargetState = new State()
                                },

                            }
                        }
                    }
                }
            };

            this.TestAddPaths(paths, expectedEntryState);
        }
        
        /// <summary>
        /// Tests that AddPath throws exception when fed with null path.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPath_NullPath_ThrowsException()
        {
            new StateMachineBuilder().AddPath(null);
        }
        
        /// <summary>
        /// Tests that single call to AddPath with a set path results in correct state machine (with the path ungrouped).
        /// </summary>
        [Test]
        public void AddPath_SingleSetPath_BuildsCorrectMachine()
        {
            var path = new ITransition[]
            {
                new SetTransition 
                {
                    GlyphIdSet = new HashSet<ushort> { 1, 2 }, 
                    HeadShift = 1, 
                    Action = new SubstitutionAction
                    {                            
                        ReplacedGlyphCount = 1, 
                        ReplacementGlyphIds = new ushort[] { 1 }
                    }
                },
                new SetTransition 
                {
                    GlyphIdSet = new HashSet<ushort> { 3, 4 }, 
                    HeadShift = 2, 
                    Action = new SubstitutionAction
                    {                            
                        ReplacedGlyphCount = 2, 
                        ReplacementGlyphIds = new ushort[] { 2 }
                    }
                }
            };

            var expectedEntryState = new State
            {
                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, //2
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }, 
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition 
                                {
                                    GlyphId = 3,
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 4,
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                }
                            }
                        }
                    },
                    new SimpleTransition 
                    {
                        GlyphId = 2,
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }, 
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition 
                                {
                                    GlyphId = 3,
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                },
                                new SimpleTransition 
                                {
                                    GlyphId = 4,
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                }
                            }
                        }
                    }
                }
            };

            this.TestAddPaths(new[] { path }, expectedEntryState);
        }

        /// <summary>
        /// Tests that single call to AddPath results in correct state machine.
        /// </summary>
        [Test]
        public void AddPath_SinglePath_BuildsCorrectMachine()
        {
            var path = new ITransition[]
            {
                new SimpleTransition {
                    GlyphId = 1, 
                    HeadShift = 1, 
                    Action = new SubstitutionAction
                    {                            
                        ReplacedGlyphCount = 1, 
                        ReplacementGlyphIds = new ushort[] { 1 }
                    }
                },
                new SimpleTransition {
                    GlyphId = 2, 
                    HeadShift = 2, 
                    Action = new SubstitutionAction
                    {                            
                        ReplacedGlyphCount = 2, 
                        ReplacementGlyphIds = new ushort[] { 2 }
                    }
                }
            };

            var expectedEntryState = new State
            {
                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }, 
                        },
                        TargetState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition {
                                    GlyphId = 2, 
                                    HeadShift = 2, 
                                    Action = new SubstitutionAction
                                    {                            
                                        ReplacedGlyphCount = 2, 
                                        ReplacementGlyphIds = new ushort[] { 2 }
                                    },
                                    TargetState = new State()
                                }
                            }
                        }
                    }
                }
            };

            this.TestAddPaths(new[] { path }, expectedEntryState);
        }

        /// <summary>
        /// Tests that GetStateMachine returns a state machine with correct lists of all states and transitions.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", 
            Justification = "Makes path definition much less readable in this case.")]
        [Test]
        public void GetStateMachine_ForkingPath_CollectsAllStatesAndTransitions()
        {
            SimpleTransition commonTransition, forkedTransition1, forkedTransition2;

            var paths = new[]
            {
                new[]
                {
                    commonTransition = new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    }, 
                    forkedTransition1 = new SimpleTransition 
                    {
                        GlyphId = 2, 
                        HeadShift = 2, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 2, 
                            ReplacementGlyphIds = new ushort[] { 2 }
                        }
                    }
                }, 
                new[]
                {
                    new SimpleTransition 
                    {
                        GlyphId = 1, 
                        HeadShift = 1, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 1, 
                            ReplacementGlyphIds = new ushort[] { 1 }
                        }
                    }, 
                    forkedTransition2 = new SimpleTransition {
                        GlyphId = 3, 
                        HeadShift = 3, 
                        Action = new SubstitutionAction
                        {                            
                            ReplacedGlyphCount = 3, 
                            ReplacementGlyphIds = new ushort[] { 3 }
                        }
                    }
                }
            };

            var builder = new StateMachineBuilder();

            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var result = builder.GetStateMachine();

            Assert.AreEqual(result.States.Count, 4);
            Assert.IsTrue(
                new[] { commonTransition, forkedTransition1, forkedTransition2 }.ValuesEqual(result.Transitions, new TransitionNonrecursiveEqualityComparer()));
        }

        // TODO: Add test for state machine which includes back-transition somewhere.

        /// <summary>
        /// Tests that UngroupPath ungroups a simple path.
        /// </summary>
        [Test]
        public void UngroupPath_SimplePath_UngroupsThePath()
        {
            var path = new ITransition[]
            {
                new SimpleTransition
                {
                    HeadShift = 1,
                    GlyphId = 1,
                    Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs
                },
                new SetTransition
                {
                    HeadShift = 2,
                    GlyphIdSet = new HashSet<ushort> { 21, 22, 23 },
                    Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs
                },
                new AlwaysTransition
                {
                    HeadShift = 3,
                    Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs
                }
            };

            var builder = new StateMachineBuilder();
            var result = builder.UngroupPath(path).ToList();

            var expectedPaths = new IEnumerable<ITransition>[]
            {
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 21,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                },
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 22,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                },
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 23,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                }
            };

            var comparer = new PathEqualityComparer();
            Assert.IsTrue(comparer.Equals(expectedPaths[0], result[0]));
            Assert.IsTrue(comparer.Equals(expectedPaths[1], result[1]));
            Assert.IsTrue(comparer.Equals(expectedPaths[2], result[2]));
            Assert.AreEqual(expectedPaths.Length, result.Count);
        }

        /// <summary>
        /// Tests that UngroupPath ungroups a simple path with back-transitions.
        /// </summary>
        [Test]
        public void UngroupPath_SimplePathWithBackTransitions_UngroupsThePath()
        {

            State state1 = new State(), state2;
            state1.Transitions.Add(new AlwaysTransition { TargetState = state1 });
            var path = new ITransition[]
            {
                new SimpleTransition
                {
                    HeadShift = 1,
                    GlyphId = 1,
                    Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs,
                    TargetState = state1
                },
                new SetTransition
                {
                    HeadShift = 2,
                    GlyphIdSet = new HashSet<ushort> { 21, 22, 23 },
                    Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs,
                    TargetState = state2 = new State(),
                },
                new AlwaysTransition
                {
                    HeadShift = 3,
                    Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                    LookupFlags = LookupFlags.IgnoreBaseGlyphs,
                    TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 } } }
                }
            };

            var builder = new StateMachineBuilder();
            var result = builder.UngroupPath(path).ToList();

            var expectedPaths = new IEnumerable<ITransition>[]
            {
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 21,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                },
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 22,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                },
                new ITransition[]
                {
                    new SimpleTransition
                    {
                        HeadShift = 1,
                        GlyphId = 1,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 1 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new SimpleTransition
                    {
                        HeadShift = 2,
                        GlyphId = 23,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 2 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    },
                    new AlwaysTransition
                    {
                        HeadShift = 3,
                        Action = new SubstitutionAction { ReplacedGlyphCount = 3 },
                        LookupFlags = LookupFlags.IgnoreBaseGlyphs
                    }
                }
            };

            var comparer = new PathEqualityComparer();
            Assert.IsTrue(comparer.Equals(expectedPaths[0], result[0]));
            Assert.IsTrue(comparer.Equals(expectedPaths[1], result[1]));
            Assert.IsTrue(comparer.Equals(expectedPaths[2], result[2]));
            Assert.AreEqual(expectedPaths.Length, result.Count);

            Assert.AreSame(result[0].First().TargetState.Transitions.First().TargetState, result[0].First().TargetState);
            Assert.AreSame(result[2].ElementAt(2).TargetState.Transitions.First().TargetState, result[2].ElementAt(1).TargetState);
        }
    }
}