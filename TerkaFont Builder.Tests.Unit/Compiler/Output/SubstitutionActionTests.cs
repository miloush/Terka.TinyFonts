namespace Terka.FontBuilder.Compiler.Output
{
    using NUnit.Framework;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="SubstitutionAction"/> class.
    /// </summary>
    [TestFixture]
    public class SubstitutionActionTests
    {
        /// <summary>
        /// Tests that Clone clones all attributes of the state.
        /// </summary>
        [Test]
        public void Clone_State_ReturnsClone()
        {
            var a = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                SkippedGlyphCount = 2,
                ReplacementGlyphIds = new ushort[] { 1, 2, 3 }
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
            var a = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                ReplacementGlyphIds = new ushort[] { 1, 2, 3 }
            };

            var b = a.Clone();

            Assert.AreNotSame(a, b);
        }
    }
}