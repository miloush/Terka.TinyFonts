namespace Terka.FontBuilder.Parser.Output
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Corresponds to OT "Coverage format 2".
    /// </summary>
    public class RangeCoverageTable : ICoverageTable
    {
        /// <summary>
        /// Gets or sets the coverage ranges.
        /// </summary>
        /// <value>
        /// The coverage ranges.
        /// </value>
        public IEnumerable<CoverageRange> CoverageRanges { get; set; }

        /// <inheritdoc />
        public IDictionary<ushort, ushort> CoveredGlyphIds
        {
            get
            {
                return 
                    (from coverageRange in this.CoverageRanges
                    from index in Enumerable.Range(0, coverageRange.MaxCoveredId - coverageRange.MinCoveredId + 1)
                    select new
                        {
                            Key = (ushort)(index + coverageRange.FirstCoveredCoverageIndex),
                            Value = (ushort)(index + coverageRange.MinCoveredId)
                        }).ToDictionary(p => p.Key, p => p.Value);
            }
        }
    }
}
