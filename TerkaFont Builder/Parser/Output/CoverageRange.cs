namespace Terka.FontBuilder.Parser.Output
{
    /// <summary>
    /// Corresponds to OT "RangeRecord".
    /// </summary>
    public class CoverageRange
    {
        /// <summary>
        /// Gets or sets the minimal covered ID.
        /// </summary>
        /// <value>
        /// The minimal covered ID.
        /// </value>
        public virtual ushort MinCoveredId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximal covered ID.
        /// </summary>
        /// <value>
        /// The maximal covered ID.
        /// </value>
        public virtual ushort MaxCoveredId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the coverage index of the first covered glyph.
        /// </summary>
        /// <value>
        /// The first index of the covered coverage.
        /// </value>
        public virtual ushort FirstCoveredCoverageIndex
        {
            get;
            set;
        }
    }
}
