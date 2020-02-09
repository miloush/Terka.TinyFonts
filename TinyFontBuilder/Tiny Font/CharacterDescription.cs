namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// Character metrics.
    /// </summary>
    public class CharacterDescription
    {
        internal const int SizeOf = sizeof(ushort) + sizeof(sbyte) + sizeof(sbyte);

        private ushort _offset;
        private sbyte _leftMargin;
        private sbyte _rightMargin;

        /// <summary>
        /// Gets or sets the horizontal offset into the bitmap for this character.
        /// </summary>
        public ushort Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        /// <summary>
        /// Gets or sets left margin for this character.
        /// </summary>
        public sbyte LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; }
        }
        /// <summary>
        /// Gets or sets right margin for this character.
        /// </summary>
        public sbyte RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; }
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

            _offset = reader.ReadUInt16();
            _leftMargin = reader.ReadSByte();
            _rightMargin = reader.ReadSByte();
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
            writer.Write(_leftMargin);
            writer.Write(_rightMargin);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("Char at {0} (margins {1}, {2})", _offset, _leftMargin, _rightMargin);
        }
    }
}
