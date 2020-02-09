namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// The range represents a sequence of all Unicode characters starting at first character up to last character inclusive. For each range, last character-first character+1 character fields will be included. 
    /// </summary>
    public class CharacterRangeDescription
    {
        internal const int SizeOf = sizeof(uint) + sizeof(char) + sizeof(char) + sizeof(uint);

        private uint _indexOfFirstCharacter;
        private char _firstCharacter;
        private char _lastCharacter;
        private uint _offset;

        /// <summary>
        /// Gets or sets zero based index of first character.
        /// </summary>
        public uint IndexOfFirstCharacter
        {
            get { return _indexOfFirstCharacter; }
            set { _indexOfFirstCharacter = value; }
        }
        /// <summary>
        /// Gets or sets Unicode code point of the first character in the range.
        /// </summary>
        public char FirstCharacter
        {
            get { return _firstCharacter; }
            set { _firstCharacter = value; }
        }
        /// <summary>
        /// Gets or sets Unicode code point of the last character in the range.
        /// </summary>
        public char LastCharacter
        {
            get { return _lastCharacter; }
            set { _lastCharacter = value; }
        }
        /// <summary>
        /// Gets or sets the horizontal offset into the bitmap for this range.
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

            _indexOfFirstCharacter = reader.ReadUInt32();
            _firstCharacter = (char)reader.ReadUInt16();
            _lastCharacter = (char)reader.ReadUInt16();
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

            writer.Write(_indexOfFirstCharacter);
            writer.Write((ushort)_firstCharacter);
            writer.Write((ushort)_lastCharacter);
            writer.Write(_offset);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (_firstCharacter < _lastCharacter)
                return string.Format("Char range {0}-{1} ({2} to {3}), offset: {4}, index: {5}", (ushort)_firstCharacter, (ushort)_lastCharacter, _firstCharacter, _lastCharacter, _offset, _indexOfFirstCharacter);
            else
                return string.Format("Char range {0} '{1}', offset: {2}, index: {3}", (ushort)_firstCharacter, _firstCharacter, _offset, _indexOfFirstCharacter);
        }
    }
}
