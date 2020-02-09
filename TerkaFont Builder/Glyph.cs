namespace Terka.FontBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Represents one positioned glyph instance.
    /// </summary>
    public class Glyph
    {
        // TODO: Staci sbyte na advance? Delalo by to problem s velmi velkymi pismeny (> 127 px).

        /// <summary>
        /// Gets or sets the ID of this glyph.
        /// </summary>
        /// <value>
        /// The glyph id.
        /// </value>
        public ushort GlyphId { get; set; }

        /// <summary>
        /// Gets or sets the advance X.
        /// </summary>
        /// <value>
        /// The advance X.
        /// </value>
        public sbyte AdvanceX { get; set; }

        /// <summary>
        /// Gets or sets the advance Y.
        /// </summary>
        /// <value>
        /// The advance Y.
        /// </value>
        public sbyte AdvanceY { get; set; }

        /// <summary>
        /// Gets or sets the offset X.
        /// </summary>
        /// <value>
        /// The offset X.
        /// </value>
        public sbyte OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the offset Y.
        /// </summary>
        /// <value>
        /// The offset Y.
        /// </value>
        public sbyte OffsetY { get; set; }

        /// <summary>
        /// Determines if two glyphs are equal.
        /// </summary>
        /// <param name="left">First glyph.</param>
        /// <param name="right">Second glyph.</param>
        /// <returns>True if the glyphs are equal, false otherwise</returns>
        public static bool operator ==(Glyph left, Glyph right)
        {
            if ((object)left == null)
            {
                return false;
            }

            if ((object)right == null)
            {
                return false;
            }
            
            return left.Equals(right);
        }

        /// <summary>
        /// Determines if two glyphs are not equal.
        /// </summary>
        /// <param name="left">First glyph.</param>
        /// <param name="right">Second glyph.</param>
        /// <returns>True if the glyphs are not equal, false otherwise</returns>
        public static bool operator !=(Glyph left, Glyph right)
        {            
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Glyph))
            {
                return false;
            }

            return this.Equals((Glyph)obj);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return this.GlyphId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compares this glyph to another glyph.
        /// </summary>
        /// <param name="other">The other glyph.</param>
        /// <returns>True if the glyphs are equal, false otherwise.</returns>
        protected bool Equals(Glyph other)
        {
            return this.GlyphId == other.GlyphId &&
                this.AdvanceX == other.AdvanceX &&
                this.AdvanceY == other.AdvanceY &&
                this.OffsetX == other.OffsetX &&
                this.OffsetY == other.OffsetY;
        }
    }
}
