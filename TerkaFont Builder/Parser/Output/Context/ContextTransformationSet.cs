namespace Terka.FontBuilder.Parser.Output.Context
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "SubstLookupRecord".
    /// </summary>
    public class ContextTransformationSet
    {
        /// <summary>
        /// Gets or sets the index of the first transformed glyph. The index is position in the matched context.
        /// </summary>
        /// <value>
        /// The index of the first transformed glyph.
        /// </value>
        public virtual int FirstGlyphIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transformations to use on the transformed sequence.
        /// </summary>
        /// <value>
        /// The transformations.
        /// </value>
        public virtual ICollection<IGlyphTransformationTable> Transformations
        {
            get;
            set;
        }
    }
}