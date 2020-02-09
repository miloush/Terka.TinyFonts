namespace Terka.FontBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a label of a feature/script/language system.
    /// List of all known tags: http://www.microsoft.com/typography/otspec/ttoreg.htm .
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontBuilder.Tag" /> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public Tag(string label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            
            if (label.Length != 4)
            {
                throw new ArgumentException("Tag label must always consist of 4 ASCII letters, numbers and or spaces.");    
            }

            this.Label = label;
        }

        /// <summary>
        /// Gets or the label label which consists of 4 ASCII letters, numbers and/or spaces.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Determines if two tags are equal.
        /// </summary>
        /// <param name="a">First tag.</param>
        /// <param name="b">Second tag.</param>
        /// <returns>True if the tags are equal, false otherwise</returns>
        public static bool operator ==(Tag a, Tag b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Label == b.Label;
        }

        /// <summary>
        /// Determines if two tags are not equal.
        /// </summary>
        /// <param name="a">First tag.</param>
        /// <param name="b">Second tag.</param>
        /// <returns>True if the tags are not equal, false otherwise</returns>
        public static bool operator !=(Tag a, Tag b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "Tag[" + this.Label + "]";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Tag))
            {
                return false;
            }

            var tag = obj as Tag;

            return tag.Label == this.Label;
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return this.Label != null ? this.Label.GetHashCode() : 0;
        }
    }
}
