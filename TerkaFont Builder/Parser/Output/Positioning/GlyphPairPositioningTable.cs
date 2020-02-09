namespace Terka.FontBuilder.Parser.Output.Positioning
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "PairPosFormat1".
    /// </summary>
    public class GlyphPairPositioningTable : CoveredGlyphTransformationTableBase
    {
        public IEnumerable<IEnumerable<PositioningPair>> PairSets { get; set; }
    }
}