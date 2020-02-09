namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    using Rhino.Mocks;

    using Terka.FontBuilder.Compiler.Output;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    // TODO: Testy na HeadShift a akce

    /// <summary>
    /// Tests for the <see cref="TransitionNonrecursiveEqualityComparer"/> class.
    /// </summary>
    [TestFixture]
    public class TransitionNonrecursiveEqualityComparerTests
    {
        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <returns>The comparer.</returns>
        public TransitionNonrecursiveEqualityComparer GetComparer()
        {
            return new TransitionNonrecursiveEqualityComparer();
        }

        /// <summary>
        /// Tests that Equals on two different set transitions returns false.
        /// </summary>
        [Test]
        public void Equals_DifferentSetTransitions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var transitionA = new SetTransition { GlyphIdSet = new HashSet<ushort> { 1, 2 } };
            var transitionB = new SetTransition { GlyphIdSet = new HashSet<ushort> { 3, 4 } };

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals on two different simple transitions returns false.
        /// </summary>
        [Test]
        public void Equals_DifferentSimpleTransitions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var transitionA = new SimpleTransition { GlyphId = 2 };
            var transitionB = new SimpleTransition { GlyphId = 1 };

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals returns false on two transitions of different types.
        /// </summary>
        [Test]
        public void Equals_DifferentTypes_ReturnsFalse()
        {
            var comparer = this.GetComparer();

            var transitionA = new SimpleTransition ();
            var transitionB = new SetTransition ();

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals on two equal set transitions returns true.
        /// </summary>
        [Test]
        public void Equals_EqualSetTransitions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var transitionA = new SetTransition { GlyphIdSet = new HashSet<ushort> { 1, 2 } };
            var transitionB = new SetTransition { GlyphIdSet = new HashSet<ushort> { 1, 2 } };

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that Equals on two equal simple transitions returns true.
        /// </summary>
        [Test]
        public void Equals_EqualSimpleTransitions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var transitionA = new SimpleTransition { GlyphId = 0 };
            var transitionB = new SimpleTransition { GlyphId = 0 };

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that Equals on two equal always transitions returns true.
        /// </summary>
        [Test]
        public void Equals_AlwaysTransitions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var transitionA = new AlwaysTransition();
            var transitionB = new AlwaysTransition();

            var result = comparer.Equals(transitionA, transitionB);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that Equals returns false when only X is nulll.
        /// </summary>
        [Test]
        public void Equals_NullX_ReturnsFalse()
        {
            Assert.IsFalse(this.GetComparer().Equals(null, new SimpleTransition()));
        }

        /// <summary>
        /// Tests that Equals returns false when only Y is null.
        /// </summary>
        [Test]
        public void Equals_NullY_ReturnsFalse()
        {
            Assert.IsFalse(this.GetComparer().Equals(new SimpleTransition(), null));
        }

        /// <summary>
        /// Tests that Equals returns true when both values are null.
        /// </summary>
        [Test]
        public void Equals_NullXAndY_ReturnsTrue()
        {
            Assert.IsTrue(this.GetComparer().Equals(null, null));
        }


        /// <summary>
        /// Tests that Equals throws exception when given unknown transition type.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Equals_UnknownStateType_ThrowsException()
        {
            var a = MockRepository.GenerateMock<ITransition>();
            var b = MockRepository.GenerateMock<ITransition>();

            Assert.IsFalse(this.GetComparer().Equals(a, b));
        }
    }
}