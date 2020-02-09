namespace Terka.FontBuilder.Parser.Output
{
    using System.Linq;
    using NUnit.Framework;
    using Terka.FontBuilder.Testing;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="ListGlyphClassDefinition"/> class.
    /// </summary>
    [TestFixture]
    public class ListGlyphClassDefinitionTests
    {
        /// <summary>
        /// Tests that ClassAssignments returns correct class definitions.
        /// </summary>
        [Test]
        public void ClassAssignments_SeveralEntries_ReturnsCorrectCLassDefinitions()
        {
            var classDef = new ListGlyphClassDefinition
            {
                FirstGlyphId = 5,
                ClassIdList = new ushort[]
                {
                    4, 5, 4, 2
                }
            };

            var expected = new IGrouping<ushort, ushort>[]
            {
                new GroupingStub<ushort, ushort>(4) { 5, 7 }, 
                new GroupingStub<ushort, ushort>(5) { 6 }, 
                new GroupingStub<ushort, ushort>(2) { 8 }
            };

            Assert.That(expected, Is.EquivalentTo(classDef.ClassAssignments));
        }
    }
}
