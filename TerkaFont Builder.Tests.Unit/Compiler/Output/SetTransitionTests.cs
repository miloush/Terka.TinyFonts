namespace Terka.FontBuilder.Compiler.Output
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="SetTransition"/> class.
    /// </summary>
    [TestFixture]
    public class SetTransitionTests
    {
        /// <summary>
        /// Tests that IsGlyphIdMatching returns true on maching glyph ID.
        /// </summary>
        [Test]
        public void IsGlyphIdMatching_MachingGlyphId_ReturnsTrue()
        {
            var transition = new SetTransition { GlyphIdSet = new HashSet<ushort> { 0, 1, 2 } };

            var result = transition.IsGlyphIdMatching(1);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that IsGlyphIdMatching returns true when called with non-maching glyph ID.
        /// </summary>
        [Test]
        public void IsGlyphIdMatching_NonmachingGlyphId_ReturnsTrue()
        {
            var transition = new SetTransition { GlyphIdSet = new HashSet<ushort> { 0, 1, 2 } };

            var result = transition.IsGlyphIdMatching(3);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that IsFallback returns false.
        /// </summary>
        [Test]
        public void IsFallback_Always_ReturnsFalse()
        {
            var transition = new SetTransition { GlyphIdSet = new HashSet<ushort> { 0, 1, 2 } };

            Assert.IsFalse(transition.IsFallback);
        }

        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_State_ReturnsClone()
        {
            var a = new SetTransition
            {
                GlyphIdSet = new HashSet<ushort> { 0, 1, 2 },
                HeadShift = 1,
                LookupFlags = LookupFlags.IgnoreBaseGlyphs,
                Action = new SubstitutionAction
                {
                    ReplacedGlyphCount = 2
                }
            };

            var b = a.Clone();

            var comparer = new TransitionNonrecursiveEqualityComparer();
            Assert.IsTrue(comparer.Equals(a, b));
        }

        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_State_ReturnsAnotherInstance()
        {
            var a = new SetTransition
            {
                GlyphIdSet = new HashSet<ushort> { 0, 1, 2 },
                HeadShift = 1,
                LookupFlags = LookupFlags.IgnoreBaseGlyphs,
                Action = new SubstitutionAction
                {
                    ReplacedGlyphCount = 2
                }
            };

            var b = a.Clone();

            Assert.AreNotSame(a, b);
        }
    }
}