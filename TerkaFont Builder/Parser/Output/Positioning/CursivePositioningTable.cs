namespace Terka.FontBuilder.Parser.Output.Positioning
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "CursivePosFormat1".
    /// </summary>
    public class CursivePositioningTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the entry exit records.
        /// </summary>
        /// <value>
        /// The entry exit records.
        /// </value>
        public IEnumerable<Tuple<AnchorPoint, AnchorPoint>> EntryExitRecords { get; set; }
    }
}
