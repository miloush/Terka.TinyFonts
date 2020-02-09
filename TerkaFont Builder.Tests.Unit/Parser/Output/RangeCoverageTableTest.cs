namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="RangeCoverageTable"/> class.
    /// </summary>
    [TestFixture]
    public class RangeCoverageTableTest
    {
        /// <summary>
        /// Tests that CoveredGlyphIds generates correct coverage pairs for two coverage ranges.
        /// </summary>
        [Test]
        public void CoveredGlyphIds_TwoRanges_ReturnsCorrectCoverage()
        {
            var table = new RangeCoverageTable
            {
                CoverageRanges = new[]
                {
                    new CoverageRange
                    {
                        FirstCoveredCoverageIndex = 5, 
                        MinCoveredId = 10, 
                        MaxCoveredId = 12
                    }, 
                    new CoverageRange
                    {
                        FirstCoveredCoverageIndex = 10, 
                        MinCoveredId = 20, 
                        MaxCoveredId = 22
                    }
                }
            };

            var expected = new Dictionary<ushort, ushort>
            {
                { 5, 10 }, 
                { 6, 11 }, 
                { 7, 12 }, 
                { 10, 20 }, 
                { 11, 21 }, 
                { 12, 22 },
            };

            Assert.That(expected, Is.EquivalentTo(table.CoveredGlyphIds));
        }
    }
}