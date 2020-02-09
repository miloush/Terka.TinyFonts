namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// Metrics for Tiny Font file.
    /// All the metric values are properties of the whole font.
    /// </summary>
    public class FontMetrics : ICloneable
    {
        internal const int SizeOf = sizeof(ushort) + sizeof(short) * 7;

        private ushort _height;
        private short _offset;
        private short _ascent;
        private short _descent;
        private short _internalLeading;
        private short _externalLeading;
        private short _averageCharWidth;
        private short _maximumCharWidth;

        /// <summary>
        /// Gets or sets cell height of the font.
        /// </summary>
        public ushort Height
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// Gets of sets number of rows not covered by the bitmap.
        /// </summary>
        public short Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        /// <summary>
        /// Gets or sets ascent.
        /// </summary>
        public short Ascent
        {
            get { return _ascent; }
            set { _ascent = value; }
        }
        /// <summary>
        /// Gets of sets descent.
        /// </summary>
        public short Descent
        {
            get { return _descent; }
            set { _descent = value; }
        }
        /// <summary>
        /// Gets or sets internal leading.
        /// </summary>
        public short InternalLeading
        {
            get { return _internalLeading; }
            set { _internalLeading = value; }
        }
        /// <summary>
        /// Gets or sets external leading.
        /// </summary>
        public short ExternalLeading
        {
            get { return _externalLeading; }
            set { _externalLeading = value; }
        }
        /// <summary>
        /// Gets or sets average character width.
        /// </summary>
        public short AverageCharacterWidth
        {
            get { return _averageCharWidth; }
            set { _averageCharWidth = value; }
        }
        /// <summary>
        /// Gets or sets maximum character width.
        /// </summary>
        public short MaximumCharacterWidth
        {
            get { return _maximumCharWidth; }
            set { _maximumCharWidth = value; }
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();

            _height = reader.ReadUInt16();
            _offset = reader.ReadInt16();
            _ascent = reader.ReadInt16();
            _descent = reader.ReadInt16();
            _internalLeading = reader.ReadInt16();
            _externalLeading = reader.ReadInt16();
            _averageCharWidth = reader.ReadInt16();
            _maximumCharWidth = reader.ReadInt16();
        }
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException();

            writer.Write(_height);
            writer.Write(_offset);
            writer.Write(_ascent);
            writer.Write(_descent);
            writer.Write(_internalLeading);
            writer.Write(_externalLeading);
            writer.Write(_averageCharWidth);
            writer.Write(_maximumCharWidth);
        }

        /// <summary>
        /// Creates a deep copy of the font metrics.
        /// </summary>
        /// <returns>A new <see cref="FontMetrics"/> instance with the same values.</returns>
        public object Clone()
        {
            return new FontMetrics
            {
                _height = _height,
                _offset = _offset,
                _ascent = _ascent,
                _descent = _descent,
                _internalLeading = _internalLeading,
                _externalLeading = _externalLeading,
                _averageCharWidth = _averageCharWidth,
                _maximumCharWidth = _maximumCharWidth
            };
        }
    }
}
