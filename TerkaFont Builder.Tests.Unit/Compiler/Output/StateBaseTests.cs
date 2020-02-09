namespace Terka.FontBuilder.Compiler.Output
{
    using NUnit.Framework;

    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="State"/> class.
    /// </summary>
    [TestFixture]
    public class StateBaseTests
    {
        /// <summary>
        /// Tests that GetTransitionByGlyphId returns null if there are no matching transition.
        /// </summary>
        [Test]
        public void GetTransitionByGlyphId_NonmatchingGlyphIdAndFeature_ReturnsNull()
        {
            var transition = MockRepository.GenerateStub<ITransition>();

            transition.Stub(p => p.IsGlyphIdMatching(1)).Return(true);

            var state = new State
            {
                Transitions = new[] { transition }
            };

            var result = state.GetTransitionByGlyphId(2);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests that GetTransitionByGlyphId returns matching transition if there is one.
        /// </summary>
        [Test]
        public void GetTransitionByGlyphId_TwoMatchingGlyphIdAndFeature_ReturnsFirstMachingTransition()
        {
            var transition1 = MockRepository.GenerateStub<ITransition>();
            var transition2 = MockRepository.GenerateStub<ITransition>();

            transition1.Stub(p => p.IsGlyphIdMatching(1)).Return(true);
            transition2.Stub(p => p.IsGlyphIdMatching(1)).Return(true);

            var state = new State
            {
                Transitions = new[] { transition1, transition2 }
            };

            var result = state.GetTransitionByGlyphId(1);

            Assert.AreSame(transition1, result);
        } 
    }
}