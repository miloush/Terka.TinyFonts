namespace Terka.FontBuilder.Parser.Output.Positioning
{
    using System;
    using System.Collections.Generic;

    public class ClassPairPositioningTable : CoveredGlyphTransformationTableBase
    {
        public virtual IGlyphClassDefinition FirstClassDef
        {
            get;
            set;
        }

        public virtual IGlyphClassDefinition SecondClassDef
        {
            get;
            set;
        }

        public IEnumerable<IEnumerable<Tuple<GlyphPositionChange, GlyphPositionChange>>> PairSets { get; set; }
    }
}

