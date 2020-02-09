namespace Terka.FontBuilder.Optimizer
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Terka.FontBuilder.Compiler;
    using Terka.FontBuilder.Compiler.Output;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="StateMachineOptimizer"/> class.
    /// </summary>
    [TestFixture]
    public class StateMachineOptimizerTests
    {
        /// <summary>
        /// Tests that GetFinalStates returns final states correctly for a state machine with two forked paths.
        /// </summary>
        [Test]
        public void GetFinalStates_StateMachineWithForkedPaths_IdentifiesEndStates()
        {
            var builder = new StateMachineBuilder();
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 0 },                     
                new SimpleTransition { GlyphId = 1 }                     
            });
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 0 },                     
                new SimpleTransition { GlyphId = 2 }                     
            });
            var machine = builder.GetStateMachine();

            var optimizer = new StateMachineOptimizer();
            var result = optimizer.GetFinalStates(machine).ToList();

            Assert.IsTrue(result.All(p => p.Transitions.Count == 0));
            Assert.IsTrue(result.Distinct().Count() == 2);
        }


        /// <summary>
        /// Tests that GetFinalStates correctly handles back-transitions.
        /// </summary>
        [Test]
        public void GetFinalStates_StateMachineWithBackTransitions_IgnoresBackTransitions()
        {
            State secondState, lastState;
            var entryState = new State
            {
                Transitions = new ITransition[]
                {
                    new SimpleTransition {
                        GlyphId = 0,
                        TargetState = secondState = new State
                        {
                            Transitions = new ITransition[]
                            {
                                new SimpleTransition {
                                    GlyphId = 0,
                                    HeadShift = 3,
                                    TargetState = lastState = new State()
                                }  
                            }
                        }
                   }
                }
            };
            lastState.Transitions.Add(new AlwaysTransition { TargetState = secondState });

            var machine = new StateMachine(entryState);

            var optimizer = new StateMachineOptimizer();
            var result = optimizer.GetFinalStates(machine).ToList();

            Assert.IsTrue(result.Single().Transitions.Single().IsFallback);
        }

        /// <summary>
        /// Tests that BuildParentStateMap builds the map correctly on a simple state machine.
        /// </summary>
        [Test]
        public void BuildParentStateMap_StateMachineWithForkedPaths_BuildsOriginMap()
        {
            var builder = new StateMachineBuilder();
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 0 },                    
                new SimpleTransition { GlyphId = 1 }
            });
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 0 }, 
                new SimpleTransition { GlyphId = 2 } 
            });
            var machine = builder.GetStateMachine();

            var optimizer = new StateMachineOptimizer();
            var originMap = optimizer.BuildParentStateMap(machine);

            Assert.AreSame(originMap[machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 0).TargetState].Single(), machine.EntryState);
            Assert.AreSame(originMap[machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 1).TargetState].Single(),
                machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 0).TargetState);
            Assert.AreSame(originMap[machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 2).TargetState].Single(),
                machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 0).TargetState);
        }

        /// <summary>
        /// Tests that GetBackTransitionsByState finds all back-transitions in the machine.
        /// </summary>
        [Test]
        public void GetBackTransitionsByState_MachineWithBackTransitions_ReturnsBackTransitions()
        {
            State state1, state2;

            var builder = new StateMachineBuilder();
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 0, TargetState = state1 = new State()},                 
                new SimpleTransition { GlyphId = 1, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 } } } }
            });
            builder.AddPath(new[]
            {
                new SimpleTransition { GlyphId = 3 },                     
                new SimpleTransition { GlyphId = 4, TargetState = state2 = new State()},  
                new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 } } } }
            });

            var optimizer = new StateMachineOptimizer();
            var machine = builder.GetStateMachine();
            var backTransitionsByState = optimizer.GetBackTransitionsByState(machine);

            Assert.AreEqual(2, backTransitionsByState.Count);
            Assert.AreSame(backTransitionsByState[machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 1).TargetState].TargetState,
                machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 0).TargetState);
            Assert.AreSame(backTransitionsByState[machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 5).TargetState].TargetState,
                machine.Transitions.OfType<SimpleTransition>().Single(p => p.GlyphId == 4).TargetState);
        }

        /// <summary>
        /// Tests that Optimize correctly merges equivalent two regions of a state machine.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRepeatingRegion_MergesTheRegions()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          -3-> X -4-> X
             *   -5-> X -2-> X
             *          -6-> X -7-> X -2-> X
             *                        -3-> X -4-> X
             * 
             * What the optimized machine should look like (2, 3, 4 part is merged):
             * 
             * X -1-------------------> X -2------------> X
             *                            -3-> X -4----->
             *   -5-> X -2------------------------------>
             *          -6-> X -7----->
             *        
             */
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 2 }  
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 3 },  
                    new SimpleTransition { GlyphId = 4 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },  
                    new SimpleTransition { GlyphId = 2 }  
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },  
                    new SimpleTransition { GlyphId = 6 },  
                    new SimpleTransition { GlyphId = 7 },  
                    new SimpleTransition { GlyphId = 2 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },  
                    new SimpleTransition { GlyphId = 6 },  
                    new SimpleTransition { GlyphId = 7 },  
                    new SimpleTransition { GlyphId = 3 },  
                    new SimpleTransition { GlyphId = 4 }  
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);    
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(6, optimizedMachine.States.Count);
            Assert.AreEqual(8, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly distinguishes two subtrees where one subtree has extra state. Tests both cases when there is a child state missing in the 
        /// compared subtree and when there is mismatching child.
        /// </summary>
        [Test]
        public void Optimize_MachineWithInvalidatingChildState_MergesOnlyValidRegions()
        {
            /* The input state machine looks like this:
             * 
             * -1-> X -2-> X
             *        -3-> X
             * -4-> X -1-> X -2-> X
             * -5-> X -1-> X -2-> X
             *               -6-> X
             * 
             * What the optimized machine should look like:
             * 
             * -1-> X -2--------> X
             *        -3-------->
             * -4-> X -1-> X -2->
             * -5-> X -1-> X -2->
             *               -6->
             * 
             */
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 2 }  
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 3 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 4 },  
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 2 }
                }, 
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },  
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 2 }
                }, 
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },  
                    new SimpleTransition { GlyphId = 1 },  
                    new SimpleTransition { GlyphId = 6 }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(7, optimizedMachine.States.Count);
            Assert.AreEqual(10, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly detects difference in transition parameter between two regions which otherwise seem equivalent.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithInvalidatingTransition_MergesOnlyValidStates()
        {
            /* The input state machine looks like this:
             * 
             * X -2-> X -5-> X
             *   -3-> X -6-> X
             * 
             * Only final states are supposed to be merged.
             * 
             */
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2 },  
                    new SimpleTransition { GlyphId = 5 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 3 },  
                    new SimpleTransition { GlyphId = 6 } 
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(4, optimizedMachine.States.Count);
            Assert.AreEqual(4, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly detects difference in transition parameter between two regions which otherwise seem equivalent. However a back-transition 
        /// goes around this transition, so no states at all can be merged.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithBackTransitionAroundInvalidatingTransition_MergesOnlyValidStates()
        {
            /* The input state machine looks like this:
             * 
             * X -2-> X -5-> X
             *          <---             
             *   -3-> X -6-> X
             *          <---
             * 
             * The machine is supposed to remain unchanged.
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2, TargetState = state1 = new State() },
                    new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 } } } }
                },                                      
                new ITransition[]                       
                {                                       
                    new SimpleTransition { GlyphId = 3, TargetState = state2 = new State() },
                    new SimpleTransition { GlyphId = 6, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 } } } }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(5, optimizedMachine.States.Count);
            Assert.AreEqual(6, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly detects difference in transition parameter between two regions which otherwise seem equivalent. However a back-transition 
        /// goes around this transition, so no states at all can be merged.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithBackTransitionToStateItself_MergesOnlyValidStates()
        {
            /* The input state machine looks like this:
             *        <-
             *        \/
             * X -2-> X -3-> X
             *        <-
             *        \/
             *   -2-> X -6->
             *  
             * The machine is supposed to remain unchanged.
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2, HeadShift = 2, TargetState = state1 = new State() },
                    new SimpleTransition { GlyphId = 3, HeadShift = 2 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 3, HeadShift = 2, TargetState = state2 = new State() },
                    new SimpleTransition { GlyphId = 6, HeadShift = 3 }
                }
            };

            state1.Transitions.Add(new AlwaysTransition { TargetState = state1, HeadShift = 1 });
            state2.Transitions.Add(new AlwaysTransition { TargetState = state2, HeadShift = 1 });

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(4, optimizedMachine.States.Count);
            Assert.AreEqual(6, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly handles back-transitions which end in a state from which another back-transition originates.
        /// This case has a distinguishing state in the are of the back-transition closer to the entry state, therefore no states should be merged.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithBackTransitionDestinationThatHasAnotherBackTransition_DoesNotMergeUnmergableSubtrees()
        {
            /* The input state machine looks like this:
             * 
             * X -2-> X -3-> X -4-> X -5-> X
             *          <----------   <---
             * X -6-> X -7-> X -4-> X -5-> X
             *          <----------   <---

             * The machine is supposed to remain unchanged.
             * 
             */
            State state1a, state1b, state2a, state2b;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2, TargetState = state1a = new State() },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = state1b = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1a } } } },
                    new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1b } } } }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 6, TargetState = state2a = new State() },
                    new SimpleTransition { GlyphId = 7 },
                    new SimpleTransition { GlyphId = 4, TargetState = state2b = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2a } } } },
                    new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2b } } } }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var machine = builder.GetStateMachine();
            var optimizedMachine = optimizer.Optimize(machine);

            Assert.AreEqual(9, optimizedMachine.States.Count);
            Assert.AreEqual(12, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly handles back-transitions which end in a state from which another back-transition originates.
        /// This case has no dsitinguishing features, therefore the subtrees should be merged.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithBackTransitionDestinationThatHasAnotherBackTransition_MergesMergableSubtrees()
        {
            /* The input state machine looks like this:
             * 
             * X -2-> X -3-> X -4-> X -5-> X
             *          <----------   <---
             * X -6-> X -3-> X -4-> X -5-> X
             *          <----------   <---
             *  
             * The machine is supposed to look like this:
             *
             * X -2-> X -3-> X -4-> X -5-> X
             *   -6->   <----------   <---
             * 
             */
            State state1a, state1b, state2a, state2b;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2, TargetState = state1a = new State() },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = state1b = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1a } } } },
                    new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1b } } } }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 6, TargetState = state2a = new State() },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = state2b = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2a } } } },
                    new SimpleTransition { GlyphId = 5, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2b } } } }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var machine = builder.GetStateMachine();
            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(machine);

            Assert.AreEqual(5, optimizedMachine.States.Count);
            Assert.AreEqual(7, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly merges equivalent two regions of a state machine (which contain backtransitions within themselves).
        /// </summary>
        [Test]
        public void Optimize_MachineWithRepeatingRegionWithBackTransitions_MergesTheRegions()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          -3-> X -4-> X
             *          <----------
             *   -5-> X -2-> X
             *          -6-> X -7-> X -2-> X
             *                        -3-> X -4-> X
             *                        <----------
             * 
             * This is what the optimized machine should look like (targets of 2 are merged, 2-3-4 regions are merged)
             * 
             * X -1---------------> X -2-> X
             *                        -3------> X -4-> X
             *                        <---------------
             *   -5-> X -2---------------> X
             *          -6-> X -7-> 
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },
                    new SimpleTransition { GlyphId = 2 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },
                    new SimpleTransition { GlyphId = 2 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },
                    new SimpleTransition { GlyphId = 6 },
                    new SimpleTransition { GlyphId = 7, TargetState = state2 = new State() },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 5 },
                    new SimpleTransition { GlyphId = 6 },
                    new SimpleTransition { GlyphId = 7 },
                    new SimpleTransition { GlyphId = 2 }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var machine = builder.GetStateMachine();
            var optimizedMachine = optimizer.Optimize(machine);

            Assert.AreEqual(7, optimizedMachine.States.Count);
            Assert.AreEqual(9, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly refuses to merge equivalent two regions which differ
        /// by target of a backtransition going from these regions.
        /// </summary>
        [Test]
        public void Optimize_MachineWithNonMatchingBackTransitions_DoesNotMerge()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          <---
             *   -3-> X -4-> X
             *          <---
             * 
             * The machine should remain unchanged.
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 2, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 3, TargetState = state2 = new State()},
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 }}}}
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var machine = builder.GetStateMachine();
            var optimizedMachine = optimizer.Optimize(machine);

            Assert.AreEqual(5, optimizedMachine.States.Count);
            Assert.AreEqual(6, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly refuses to merge equivalent two regions with backtransitions differed by one state having a selfbacktransition in the 
        /// backtransition's destination.
        /// </summary>
        [Test]
        public void Optimize_MachineWithBackTransitionsAndSingleSelfBackTransition_DoesNotMerge()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          <---
             *   -3-> X -2-> X
             *          <---
             *        /\
             *        <-
             * 
             * The machine should remain unchanged.
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 2, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1  }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 3, TargetState = state2 = new State() },
                    new SimpleTransition { GlyphId = 2, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 }}}}
                }
            };

            state2.Transitions.Add(new AlwaysTransition { TargetState = state2, HeadShift = 1 });

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var machine = builder.GetStateMachine();
            var optimizedMachine = optimizer.Optimize(machine);

            Assert.AreEqual(5, optimizedMachine.States.Count);
            Assert.AreEqual(7, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly refuses to merge equivalent two regions which differ
        /// by target of a backtransition going from these regions.
        /// </summary>
        [Test]
        public void Optimize_MachineWithRegionsWithAndWithoutBackTransition_DoesNotMerge()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          <---
             *   -3-> X -4-> X
             *
             * The machine should remain unchanged.
             * 
             */
            State state1;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 2, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 }}}}
                },                                     
                new ITransition[]                      
                {                                      
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 2 }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(5, optimizedMachine.States.Count);
            Assert.AreEqual(5, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly refuses to merge two regions with back-transitions, where one has one extra state.
        /// </summary>
        [Test]
        public void Optimize_MachineWithNonMatchingSubtrees_DoesNotValidateBackTransition()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -2-> X
             *          <---
             *   -3-> X -3-> X
             *          <---
             *          -5-> X
             *
             * The machine should remain unchanged.
             * 
             */
            State state1, state2;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 3, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 4, TargetState = state2 = new State()},
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 3, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 4 },
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 5 }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(8, optimizedMachine.States.Count);
            Assert.AreEqual(9, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize on state machine witch three at first equivalent looking regions with backtransitions correctly recognizes
        /// that the template region is different and only merges the latter two equivalent regions.
        /// </summary>
        [Test]
        public void Optimize_MachineWithThreeBackTransitionRegionsAndMismatchingTemplate_OnlyMergesMatchingRegions()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -5-> X -3-> X -4-> X
             *          <-----------------
             * X -6-> X -2-> X -3-> X -4-> X
             *          <-----------------
             * X -7-> X -2-> X -3-> X -4-> X
             *          <-----------------
             * 
             * The optimized should look like this:
             * 
             * X -1-> X -5-> X -3-> X -4-> X
             *          <-----------------
             * X -4-> X -2-> X -3-> X -4-> X
             *   -7->   <-----------------
             *  
             */
            State state1, state2, state3;
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1, TargetState = state1 = new State()},
                    new SimpleTransition { GlyphId = 5 },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state1 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 6, TargetState = state2 = new State()},
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state2 }}}}
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 7, TargetState = state3 = new State()},
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 3 },
                    new SimpleTransition { GlyphId = 4, TargetState = new State { Transitions = new ITransition[] { new AlwaysTransition { TargetState = state3 }}}}
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(9, optimizedMachine.States.Count);
            Assert.AreEqual(11, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        /// <summary>
        /// Tests that Optimize correctly detects difference in transition parameter between two regions which otherwise seem equivalent.
        /// </summary>
        [Test]
        public void Optimize_MachineWithStatesWithChildrenInDifferentOrder_MergesEquivalentStates()
        {
            /* The input state machine looks like this:
             * 
             * X -1-> X -5-> X
             *          -6-> X
             *   -2-> X -6-> X
             *          -5-> X
             *          
             * Optimized machine should look like this:
             * 
             * X -1-> X -5-> X
             *   -2->   -6->
             * 
             */
            var paths = new[]
            {
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },
                    new SimpleTransition { GlyphId = 5 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 1 },
                    new SimpleTransition { GlyphId = 6 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 6 }
                },
                new ITransition[]
                {
                    new SimpleTransition { GlyphId = 2 },
                    new SimpleTransition { GlyphId = 5 }
                }
            };

            var builder = new StateMachineBuilder();
            foreach (var path in paths)
            {
                builder.AddPath(path);
            }

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(builder.GetStateMachine());

            Assert.AreEqual(3, optimizedMachine.States.Count);
            Assert.AreEqual(4, optimizedMachine.Transitions.Count);
            foreach (var path in paths)
            {
                this.AssertStateMachineHasPath(optimizedMachine, path);
            }
        }

        private void AssertStateMachineHasPath(StateMachine machine, ITransition[] path)
        {
            var transitionComparer = new TransitionNonrecursiveEqualityComparer();
            var currentState = machine.EntryState;
            int i = 0;
            var visitedStates = new List<State> { currentState }; // For validation of back-transitions
            foreach (var step in path)
            {
                var currentTransition = currentState.Transitions.SingleOrDefault(p => transitionComparer.Equals(step, p));

                Assert.NotNull(currentTransition, "Transition mismatch in path " + i + " after state " + currentState);

                currentState = currentTransition.TargetState;

                visitedStates.Add(currentState);

                // Validate back-transition if the state has one
                if (step.TargetState != null && step.TargetState.Transitions.Count > 0)
                {
                    var backTransition = step.TargetState.Transitions.First();
                    var targetStepIndex = path.TakeWhile(p => p.TargetState != backTransition.TargetState).Count();

                    var foundBackTransition = currentState.Transitions.SingleOrDefault(p => p.TargetState == visitedStates[targetStepIndex + 1]);

                    Assert.IsNotNull(foundBackTransition, "Back-transition expected by step " + i + " in state " + currentState + " not found in the machine (or target mismatch).");

                    Assert.IsTrue(transitionComparer.Equals(backTransition, foundBackTransition), "Back-transition not equal to expected back-transition in step " + i + " in state " + currentState + ".");
                }

                i++;
            }
        }
    }
}
