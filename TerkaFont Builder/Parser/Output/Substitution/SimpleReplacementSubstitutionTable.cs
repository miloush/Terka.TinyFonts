namespace Terka.FontBuilder.Parser.Output.Substitution
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Single substitution format 1".
    /// </summary>
    public class SimpleReplacementSubstitutionTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the collection of replacement glyph IDs. The collection is indexed by coverage indices of input glyphs.
        /// </summary>
        /// <value>
        /// The replacement glyph IDs.
        /// </value>
        public virtual IEnumerable<ushort> ReplacementGlyphIds
        {
            get;
            set;
        }
    }
}