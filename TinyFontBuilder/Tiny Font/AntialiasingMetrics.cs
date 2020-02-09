namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    /// <summary>
    /// Anti-aliasing metrics.
    /// </summary>
    public class AntialiasingMetrics
    {
        internal const int SizeOf = sizeof(uint);

        private uint _size;
        
        /// <summary>
        /// Gets or sets number of bytes of the anti-aliasing data included.
        /// </summary>
        public uint Size
        {
            get { return _size; }
            set { _size = value; }
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

            _size = reader.ReadUInt32();
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

            writer.Write(_size);
        }
    }
}
