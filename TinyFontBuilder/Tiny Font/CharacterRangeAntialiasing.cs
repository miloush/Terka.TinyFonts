namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// Character range anti-aliasing offset.
    /// </summary>
    public class CharacterRangeAntialiasing
    {
        internal const int SizeOf = sizeof(uint);

        private uint _offset;

        /// <summary>
        /// Gets the offset into the anti-aliasing data in range (in bytes).
        /// </summary>
        public uint Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _offset = reader.ReadUInt32();
        }
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(_offset);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("Char range anti-alias at {0}", _offset);
        }
    }
}
