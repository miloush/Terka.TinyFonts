namespace Terka.TinyFonts
{
    using System;
    using System.IO;

    internal class UnknownAppendix : FontAppendix
    {
        private const int MaximumAppendixSize = 0xFFFFFF;

        private int _size;
        private byte[] _data;

        /// <summary>
        /// Gets total size of this appendix.
        /// </summary>
        public override int GetSize(TinyFont font)
        {
            return _data == null ? 0 : _data.Length;
        }
        /// <summary>
        /// Gets or sets raw appendix data.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Creates new instance of unknown appendix.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        public UnknownAppendix(byte id, int size) : base(id)
        {
            _size = size;
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public override void ReadFrom(BinaryReader reader, TinyFont font)
        {
            _data = reader.ReadBytes(_size);
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public override void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (_data != null)
                writer.Write(_data);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            if (_data != null && _data.Length > MaximumAppendixSize)
                throw new InvalidOperationException();
        }
    }
}
