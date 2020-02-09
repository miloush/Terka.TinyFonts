namespace Terka.FontBuilder.Parser.Output.Positioning
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "MarkBasePOsFormat1".
    /// </summary>
    public class MarkToBasePositioningTable : IGlyphTransformationTable
    {
        /// <inheritdoc />
        public LookupFlags LookupFlags { get; set; }

        /// <summary>
        /// Gets or sets the mark glyph coverage table.
        /// </summary>
        /// <value>
        /// The mark coverage.
        /// </value>
        public ICoverageTable MarkCoverage { get; set; }

        /// <summary>
        /// Gets or sets the base glyph coverage coverage.
        /// </summary>
        /// <value>
        /// The base coverage.
        /// </value>
        public ICoverageTable BaseCoverage { get; set; }

        /// <summary>
        /// Gets or sets the mark anchor points with their respective classes.
        /// </summary>
        /// <value>
        /// The mark anchor points.
        /// </value>
        public IEnumerable<Tuple<ushort, AnchorPoint>> MarkAnchorPoints { get; set; }

        /// <summary>
        /// Gets or sets the base glyph anchor points. The outer collection is indexed by coverage index and the inner collection is indexed by class index.
        /// </summary>
        /// <value>
        /// The base glyph anchor points.
        /// </value>
        public IEnumerable<IEnumerable<AnchorPoint>> BaseAnchorPoints { get; set; }
    }
}
