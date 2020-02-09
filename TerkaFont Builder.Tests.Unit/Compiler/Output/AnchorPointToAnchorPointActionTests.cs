namespace Terka.FontBuilder.Compiler.Output
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="AnchorPointToAnchorPointAction"/> class.
    /// </summary>
    [TestFixture]
    public class AnchorPointToAnchorPointActionTests
    {
        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_Action_ReturnsClone()
        {
            var a = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 3 },
            };

            var b = a.Clone();

            var comparer = new TransitionActionEqualityComparer();
            Assert.IsTrue(comparer.Equals(a, b));
        }

        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_State_ReturnsAnotherInstance()
        {
            var a = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 2, Y = 3 },
            };

            var b = a.Clone();

            Assert.AreNotSame(a, b);
        }
    }
}