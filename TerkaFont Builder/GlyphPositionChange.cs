namespace Terka.FontBuilder
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents values added to glyph's positioning values when an positioning transformation operation commences. Corresponds to OT "ValueRecord".
    /// </summary>
    public class GlyphPositionChange
    {
        /// <summary>
        /// Gets or sets the change of advance X.
        /// </summary>
        /// <value>
        /// The advance X change.
        /// </value>
        public virtual short AdvanceX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the change of advance Y.
        /// </summary>
        /// <value>
        /// The advance Y change.
        /// </value>
        public virtual short AdvanceY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the change of offset X.
        /// </summary>
        /// <value>
        /// The offset X change.
        /// </value>
        public virtual short OffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the change of offset Y.
        /// </summary>
        /// <value>
        /// The offset Y change.
        /// </value>
        public virtual short OffsetY
        {
            get;
            set;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>True if the two instances are equal.</returns>
        public static bool operator ==(GlyphPositionChange left, GlyphPositionChange right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Compares two instances for non-equality.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>True if the two instances are not equal.</returns>
        public static bool operator !=(GlyphPositionChange left, GlyphPositionChange right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((GlyphPositionChange)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(149, this.AdvanceX, this.AdvanceY, this.OffsetX, this.OffsetY);
        }

        /// <summary>
        /// Compares this instance to another instance.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if the two instances are equal.</returns>
        protected bool Equals(GlyphPositionChange other)
        {
            return
                this.AdvanceX == other.AdvanceX &&
                this.AdvanceY == other.AdvanceY &&
                this.OffsetX == other.OffsetX &&
                this.OffsetY == other.OffsetY;
        }
    }
}