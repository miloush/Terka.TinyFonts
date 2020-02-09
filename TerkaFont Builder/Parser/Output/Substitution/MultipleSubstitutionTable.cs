namespace Terka.FontBuilder.Parser.Output.Substitution
{
    using System.Collections.Generic;

    /// <summary>
    /// Corresponds to OT "Multiple substitution subtable".
    /// </summary>
    public class MultipleSubstitutionTable : CoveredGlyphTransformationTableBase
    {
        /// <summary>
        /// Gets or sets the replacement glyph ID sequences.
        /// </summary>
        /// <value>
        /// The replacement sequences.
        /// </value>
        public virtual IEnumerable<IEnumerable<ushort>> ReplacementSequences
        {
            get;
            set;
        }
    }
}