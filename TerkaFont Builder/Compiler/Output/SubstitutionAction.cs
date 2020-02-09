namespace Terka.FontBuilder.Compiler.Output
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    public class SubstitutionAction : ITransitionAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionAction" /> class.
        /// </summary>
        public SubstitutionAction()
        {
            this.ReplacedGlyphCount = 0;
            this.ReplacementGlyphIds = new List<ushort>();
        }

        /// <summary>
        /// Gets or sets the number of glyps to the left of the head (current glyph included) which will be replaced when this state is entered.
        /// </summary>
        /// <value>
        /// The number of replaced glyphs.
        /// </value>
        public uint ReplacedGlyphCount { get; set; }

        /// <summary>
        /// Gets or sets the number of glyph skipped before glyphs start being replaced.
        /// </summary>
        /// <value>
        /// The skipped glyph count.
        /// </value>
        public int SkippedGlyphCount { get; set; }

        /// <summary>
        /// Gets or sets the collecion of glyph IDs which will be use as replacements for removed glyphs. The number of glyphs in this collection may not be equal to <see cref="ReplacedGlyphCount"/>.
        /// </summary>
        /// <value>
        /// The collection of replacement glyph IDs.
        /// </value>
        public IEnumerable<ushort> ReplacementGlyphIds { get; set; }

        /// <inheritdoc />
        public ITransitionAction Clone()
        {
            return new SubstitutionAction
            {
                ReplacedGlyphCount = this.ReplacedGlyphCount, 
                ReplacementGlyphIds = this.ReplacementGlyphIds,
                SkippedGlyphCount = this.SkippedGlyphCount
            };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(
                257,
                typeof(SubstitutionAction),
                this.ReplacedGlyphCount,
                this.ReplacementGlyphIds,
                this.SkippedGlyphCount
            );
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "([" + this.ReplacedGlyphCount + "x] -> [" + string.Join(" ", this.ReplacementGlyphIds.Select(p => p.ToString(CultureInfo.InvariantCulture))) + "])";
        }
    }
}