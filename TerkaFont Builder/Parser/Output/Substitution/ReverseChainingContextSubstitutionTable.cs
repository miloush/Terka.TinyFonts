namespace Terka.FontBuilder.Parser.Output.Substitution
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Reverse chaining contextual single substitution subtable".
    /// </summary>
    public class ReverseChainingContextSubstitutionTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the substitute glyph IDs. The collection is indexed by coverage indices of input glyphs.
        /// </summary>
        /// <value>
        /// The substitute glyph IDs.
        /// </value>
        public virtual IEnumerable<ushort> SubstituteGlyphIds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lookahead coverages. The coverages are in logical glyph order.
        /// </summary>
        /// <value>
        /// The lookahead coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> LookaheadCoverages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lookback coverages. The coverages are in reverse logical glyph order.
        /// </summary>
        /// <value>
        /// The lookback coverages.
        /// </value>
        public virtual IEnumerable<ICoverageTable> LookbackCoverages
        {
            get;
            set;
        }
    }
}