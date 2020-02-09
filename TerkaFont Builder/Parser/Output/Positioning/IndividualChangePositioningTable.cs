namespace Terka.FontBuilder.Parser.Output.Positioning
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "SinglePosFormat2 subtable".
    /// </summary>
    public class IndividualChangePositioningTable : CoveredGlyphTransformationTableBase
    {
        public virtual IEnumerable<GlyphPositionChange> PositionChanges
        {
            get;
            set;
        }

    }
}

