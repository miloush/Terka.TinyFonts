namespace Terka.FontBuilder
{
    /// <summary>
    /// Represents positioning anchor point.
    /// </summary>
    public class AnchorPoint
    {
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public short X { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public short Y { get; set; }

        /// <summary>
        /// Compares rwo anchor points.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// True if the two anchor points are equal, false otherwise.
        /// </returns>
        public static bool operator ==(AnchorPoint left, AnchorPoint right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Compares rwo anchor points.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// False if the two anchor points are equal, true otherwise.
        /// </returns>
        public static bool operator !=(AnchorPoint left, AnchorPoint right)
        {
            return !(left == right);
        }        

        /// <inheritdoc />
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

            return this.Equals((AnchorPoint)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(73, this.X, this.Y);
        }

        /// <summary>
        /// Compares this anchor point to another anchor point.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if the two anchor points are equal, false otherwise.</returns>
        protected bool Equals(AnchorPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "[X: " + this.X + ", Y: " + this.Y + "]";
        }
    }
}
