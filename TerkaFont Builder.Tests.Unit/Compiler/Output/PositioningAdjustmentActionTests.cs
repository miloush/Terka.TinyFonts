namespace Terka.FontBuilder.Compiler.Output
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class PositioningAdjustmentActionTests
    {
        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_State_ReturnsClone()
        {
            var a = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange { AdvanceX = 1, AdvanceY = 2, OffsetX = 3, OffsetY = 4 },
                    new GlyphPositionChange { AdvanceX = 2, AdvanceY = 3, OffsetX = 4, OffsetY = 5 }
                }
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
            var a = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange { AdvanceX = 1, AdvanceY = 2, OffsetX = 3, OffsetY = 4 },
                    new GlyphPositionChange { AdvanceX = 2, AdvanceY = 3, OffsetX = 4, OffsetY = 5 }
                }
            };

            var b = a.Clone();

            Assert.AreNotSame(a, b);
        }
    }
}
