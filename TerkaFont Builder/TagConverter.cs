namespace Terka.FontBuilder
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Converts tags between different formats.
    /// </summary>
    public class TagConverter
    {
        /// <summary>
        /// Converts <see cref="uint"/> to <see cref="Tag"/>.
        /// </summary>
        /// <param name="uintTag">The tag in uint format.</param>
        /// <returns>The tag in <see cref="Tag"/> format.</returns>
        public static Tag TagFromUint(uint uintTag)
        {
            return new Tag(Encoding.ASCII.GetString(BitConverter.GetBytes(uintTag).Reverse().ToArray()));
        }

        /// <summary>
        /// Converts <see cref="Tag" /> to <see cref="uint" />.
        /// </summary>
        /// <param name="tag">The tag in <see cref="Tag"/> format.</param>
        /// <returns>
        /// The tag in <see cref="uint" /> format.
        /// </returns>
        public static uint UintFromTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            return BitConverter.ToUInt32(Encoding.ASCII.GetBytes(tag.Label).Reverse().ToArray(), 0);
        }
    }
}