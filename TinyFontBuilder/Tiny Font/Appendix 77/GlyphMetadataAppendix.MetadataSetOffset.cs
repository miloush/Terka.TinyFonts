using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    partial class GlyphMetadataAppendix
    {
        /// <summary>
        /// How many bits are reserved for each character in metadata set.
        /// </summary>
        public enum MetadataSetBitLength
        {
            /// <summary>
            /// One bit per character.
            /// </summary>
            One = 0,
            /// <summary>
            /// Two bits per character.
            /// </summary>
            Two = 1,
            /// <summary>
            /// Four bits per character.
            /// </summary>
            Four = 2,
            /// <summary>
            /// One byte per character.
            /// </summary>
            Eight = 3
        }

        /// <summary>
        /// Basic characteristics for one metadata set. Each metadata set contains information for each character in font.
        /// </summary>
        public class MetadataSetOffset : IComparable<MetadataSetOffset>
        {
            internal const int SizeOf = sizeof(ushort) + sizeof(byte) * 2;

            private byte _id;
            private MetadataSetBitLength _bits;
            private int _offset;

            /// <summary>
            /// Gets or sets byte identifier for this metadata set.
            /// </summary>
            public byte Id
            {
                get { return _id; }
                set { _id = value; }
            }

            /// <summary>
            /// Gets or sets how many bits of data are used for each character.
            /// </summary>
            public MetadataSetBitLength Bits
            {
                get { return _bits; }
                set { _bits = value; }
            }
            /// <summary>
            /// Gets or sets metadata offset in bytes in appendix.
            /// </summary>
            public int Offset
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

                int header = reader.ReadInt32();

                _id = (byte)header;
                _bits = (MetadataSetBitLength)((header >> 8) & 3);
                _offset = header >> 10;
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

                int header = _id + ((int)_bits << 8) + (_offset << 10);
                writer.Write(header);
            }

            int IComparable<MetadataSetOffset>.CompareTo(MetadataSetOffset other)
            {
                return Id.CompareTo(other.Id);
            }
        }
    }
}
