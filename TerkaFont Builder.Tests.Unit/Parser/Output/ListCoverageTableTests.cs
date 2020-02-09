namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="ListCoverageTable"/> class.
    /// </summary>
    [TestFixture]
    public class ListCoverageTableTests
    {
        /// <summary>
        /// Tests that CoveredGlyphIds returns correct coverage pairs.
        /// </summary>
        [Test]
        public void CoveredGlyphIds_SeveralEntries_ReturnsCorrectCoveragePairs()
        {
            var table = new ListCoverageTable
            {
                CoveredGlyphIdList = new ushort[] { 5, 9, 12, 20 }
            };

            var expected = new Dictionary<ushort, ushort>
            {
                { 0, 5 }, 
                { 1, 9 }, 
                { 2, 12 }, 
                { 3, 20 }
            };

            Assert.That(expected, Is.EquivalentTo(table.CoveredGlyphIds));
        }
    }
}
