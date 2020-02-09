namespace Terka.TinyBitmaps
{
    using System;
    using System.IO;

    /// <summary>
    /// Bitmap Description.
    /// </summary>
    public class BitmapDescription
    {
        internal const int SizeOf = sizeof(uint) + sizeof(uint) + sizeof(BitmapDescriptionFlags) + sizeof(byte) + sizeof(BitmapType);

        private uint _width;
        private uint _height;
        private BitmapDescriptionFlags _flags;
        private byte _bitsPerPixel;
        private BitmapType _type;

        /// <summary>
        /// Gets or sets width in pixel units.
        /// </summary>
        public uint Width
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// Gets or sets height in pixel units.
        /// </summary>
        public uint Height
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// Gets or sets various flags of the bitmap.
        /// </summary>
        public BitmapDescriptionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }
        /// <summary>
        /// Gets or sets bits per pixel.
        /// </summary>
        public byte BitsPerPixel
        {
            get { return _bitsPerPixel; }
            set { _bitsPerPixel = value; }
        }
        /// <summary>
        /// Gets or sets format of the bitmap data.
        /// </summary>
        public BitmapType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _width = reader.ReadUInt32();
            _height = reader.ReadUInt32();
            _flags = (BitmapDescriptionFlags)reader.ReadUInt16();
            _bitsPerPixel = reader.ReadByte();
            _type = (BitmapType)reader.ReadByte();
        }
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(_width);
            writer.Write(_height);
            writer.Write((ushort)_flags);
            writer.Write(_bitsPerPixel);
            writer.Write((byte)_type);
        }

        #region Flags

        /// <summary>
        /// Gets or sets compressed flag.
        /// </summary>
        public bool IsCompressed
        {
            get { return _flags.HasFlag(BitmapDescriptionFlags.Compressed); }
            set { _flags = (BitmapDescriptionFlags)Helper.SetFlag((int)_flags, (int)BitmapDescriptionFlags.Compressed, value); }
        }

        #endregion
    }
}
