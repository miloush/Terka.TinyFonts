namespace Terka.FontBuilder.Optimizer
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Terka.FontBuilder.Compiler;
    using Terka.FontBuilder.Compiler.Output;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="StateMachineNormalizer"/> class.
    /// </summary>
    [TestFixture]
    public class StateMachineNormalizerTests
    {
        public TransitionNonrecursiveEqualityComparer TransitionComparer
        {
            get
            {
                return new TransitionNonrecursiveEqualityComparer();
            }
        }

        /// <summary>
        /// Tests that Normalize merges and orders transitions correctly.
        /// </summary>
        [Test]
        public void Normalize_SimpleMachine_MergesAndOrdersCorrectly()
        {

            State state1;
            var machine = new StateMachine(new State
            {

                Transitions = new ITransition[]
                {
                    new SimpleTransition 
                    { 
                        GlyphId = 1, 
                        HeadShift = 1,
                        TargetState = state1 = new State() 
                    },
                    new SimpleTransition 
                    { 
                        GlyphId = 2, 
                        HeadShift = 1,
                        TargetState = state1 
                    },
                    new SimpleTransition 
                    { 
                        GlyphId = 3, 
                        HeadShift = 1,
                        TargetState = state1 
                    },
                    new AlwaysTransition { TargetState = state1 }
                }
            });

            var normalizer = new StateMachineNormalizer();
            var normalizedMachine = normalizer.Normalize(machine);

            Assert.AreEqual(2, normalizedMachine.Transitions.Count);
            Assert.IsTrue(this.TransitionComparer.Equals(normalizedMachine.EntryState.Transitions[0], new SetTransition { HeadShift = 1, GlyphIdSet = new HashSet<ushort> { 1, 2, 3 } }));
            Assert.IsTrue(this.TransitionComparer.Equals(normalizedMachine.EntryState.Transitions[1], new AlwaysTransition ()));
        }

        /// <summary>
        /// Tests that Normalize merges and orders transitions correctly.
        /// </summary>
        [Test]
        public void Normalize_MoreComplexMachine_MergesAndOrdersCorrectly()
        {
            /*
             * The original state machine looks like this (numbers are state head shifts):
             * 
             *         <--
             *   -------->
             * 1 --> 2 --> 3 --> 4
             *   -->   -->   -->             
             *         <--------
             * 
             * The normalized machine should look like this:
             * 
             * 1 --> 2 --> 3 --> 4
             *         <--
             *         <--------
             */
            var state1 = new State();
            var state2 = new State();
            var state3 = new State();
            var state4 = new State();

            state1.Transitions = new ITransition[]
            {
                new SimpleTransition { GlyphId = 1, TargetState = state3 },
                new SimpleTransition { GlyphId = 2, TargetState = state2 },
                new SimpleTransition { GlyphId = 3, TargetState = state2 }
            };

            state2.Transitions = new ITransition[]
            {
                new SimpleTransition { GlyphId = 4, TargetState = state3 },
                new SimpleTransition { GlyphId = 5, TargetState = state3 }
            };

            state3.Transitions = new ITransition[]
            {
                new AlwaysTransition { TargetState = state2 },
                new SimpleTransition { GlyphId = 6, TargetState = state4 },
                new SimpleTransition { GlyphId = 7, TargetState = state4 }
            };

            state4.Transitions = new ITransition[]
            {
                new AlwaysTransition { TargetState = state2 } 
            };

            var machine = new StateMachine(state1);

            var normalizer = new StateMachineNormalizer();
            var normalizedMachine = normalizer.Normalize(machine);

            Assert.AreEqual(6, normalizedMachine.Transitions.Count);
            Assert.IsTrue(normalizedMachine.EntryState.Transitions.OfType<SimpleTransition>().Count(p => p.GlyphId == 1) == 1);
            Assert.IsTrue(this.TransitionComparer.Equals(normalizedMachine.EntryState.Transitions.OfType<SetTransition>().Single(), new SetTransition { GlyphIdSet = new HashSet<ushort> { 2, 3 }}));
            Assert.IsTrue(normalizedMachine.Transitions.OfType<SetTransition>().Count(p => this.TransitionComparer.Equals(p, new SetTransition { GlyphIdSet = new HashSet<ushort> { 4, 5 } })) == 1);
            Assert.IsTrue(normalizedMachine.Transitions.OfType<SetTransition>().Count(p => this.TransitionComparer.Equals(p, new SetTransition { GlyphIdSet = new HashSet<ushort> { 6, 7 } })) == 1);
            Assert.IsTrue(normalizedMachine.Transitions.OfType<AlwaysTransition>().Count() == 2);
        }

        /// <summary>
        /// Tests that Normalize skips transition which are excluded from the machine by the filtering list.
        /// </summary>
        [Test]
        public void NormalizeMachineWithSkippedTransition_SkipsTransition()
        {
            /*
             * The original state machine looks like this (numbers are state head shifts):
             * 
             *         <--
             *   -------->
             * 1 --> 2 --> 3 --> 4
             *   -->   -->   -->             
             *         <--------
             * 
             * The normalized machine should look like this (transitions 6 are 7 are not in the allowed list):
             * 
             *   --------->
             * 1 --> 2  --> 3
             *          <--
             */
            var state1 = new State();
            var state2 = new State();
            var state3 = new State();
            var state4 = new State();

            state1.Transitions = new ITransition[]
            {
                new SimpleTransition { GlyphId = 1, TargetState = state3 },
                new SimpleTransition { GlyphId = 2, TargetState = state2 },
                new SimpleTransition { GlyphId = 3, TargetState = state2 }
            };

            state2.Transitions = new ITransition[]
            {
                new SimpleTransition { GlyphId = 4, TargetState = state3 },
                new SimpleTransition { GlyphId = 5, TargetState = state3 }
            };

            state3.Transitions = new ITransition[]
            {
                new AlwaysTransition { TargetState = state2 },
                new SimpleTransition { GlyphId = 6, TargetState = state4 },
                new SimpleTransition { GlyphId = 7, TargetState = state4 }
            };

            state4.Transitions = new ITransition[]
            {
                new AlwaysTransition { TargetState = state2 } 
            };

            var machine = new StateMachine(state1);

            var normalizer = new StateMachineNormalizer();
            var normalizedMachine = normalizer.Normalize(machine, new ushort[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(4, normalizedMachine.Transitions.Count);
            Assert.AreEqual(3, normalizedMachine.States.Count);
        }
    }
}