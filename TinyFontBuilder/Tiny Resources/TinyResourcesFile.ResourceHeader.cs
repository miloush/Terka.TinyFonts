namespace Terka.TinyResources
{
    using System;
    using System.IO;

    partial class TinyResourcesFile
    {
        /// <summary>
        /// Tiny Resource data header.
        /// </summary>
        public class ResourceHeader
        {
            internal const uint RequiredSize = sizeof(ushort) + 2 * sizeof(byte) + sizeof(uint);
            
            private const byte SizeOfPaddingMask = 0x07;
            private const byte SizeOfPaddingShift = 0;

            private short _id;
            private TinyResourceKind _kind;
            private byte _flags;
            private uint _size;

            /// <summary>
            /// Gets or sets the Tiny Resource identifier.
            /// </summary>
            public short ID
            {
                get { return _id; }
                set { _id = value; }
            }
            /// <summary>
            /// Gets or sets the type of Tiny Resource. 
            /// </summary>
            public TinyResourceKind Kind
            {
                get { return _kind; }
                set { _kind = value; }
            }
            /// <summary>
            /// Gets or sets the various flags of the Tiny Resource.
            /// </summary>
            public byte Flags
            {
                get { return _flags; }
                set { _flags = value; }
            }
            /// <summary>
            /// Gets or sets the number of bytes of the Tiny Resource data.
            /// </summary>
            public uint Size
            {
                get { return _size; }
                set { _size = value; }
            }
            /// <summary>
            /// Gets or sets the 3-bit number of bytes of the padding appended to the Tiny Resource data.
            /// </summary>
            public byte SizeOfPadding
            {
                get { return (byte)Helper.UnpackFlags(SizeOfPaddingMask, SizeOfPaddingShift, _flags); }
                set { _flags = (byte)Helper.PackFlags(SizeOfPaddingMask, SizeOfPaddingShift, _flags, value); }
            }

            /// <summary>
            /// Creates a blank instance of the <see cref="ResourceHeader"/> class.
            /// </summary>
            public ResourceHeader()
            {

            }
            /// <summary>
            /// Creates a new instance of the <see cref="ResourceHeader"/> class with specified <see cref="TinyResourceKind"/>.
            /// </summary>
            /// <param name="kind">The <see cref="TinyResourceKind"/> of the resource.</param>
            public ResourceHeader(TinyResourceKind kind) : this(kind, 0)
            {

            }
            /// <summary>
            /// Creates a new instance of the <see cref="ResourceHeader"/> class with specified <see cref="TinyResourceKind"/> and ID.
            /// </summary>
            /// <param name="kind">The <see cref="TinyResourceKind"/> of the resource.</param>
            /// <param name="id">The ID of the resource.</param>
            public ResourceHeader(TinyResourceKind kind, short id)
            {
                _kind = kind;
                _id = id;
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            /// <param name="fileHeader">Optional <see cref="FileHeader"/> that specifies padding to skip after reading.</param>
            /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
            public virtual void ReadFrom(BinaryReader reader, FileHeader fileHeader = null)
            {
                if (reader == null)
                    throw new ArgumentNullException();

                _id = reader.ReadInt16();
                _kind = (TinyResourceKind)reader.ReadByte();
                _flags = (byte)reader.ReadByte();
                _size = reader.ReadUInt32();

                if (fileHeader != null)
                    reader.ReadBytes((int)(fileHeader.SizeOfResourceHeader - RequiredSize));
            }
            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
            public virtual void WriteTo(BinaryWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException();

                writer.Write(_id);
                writer.Write((byte)_kind);
                writer.Write((byte)_flags);
                writer.Write(_size);
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public virtual void Update()
            {

            }
        }
    }
}
