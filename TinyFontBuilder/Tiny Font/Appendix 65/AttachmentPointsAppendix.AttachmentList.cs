using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    public partial class AttachmentPointsAppendix
    {
        /// <summary>
        /// List of attachment points.  
        /// </summary>
        public class AttachmentList
        {
            private ushort _count;
            private List<AttachmentPoint> _points;

            /// <summary>
            /// Gets or sets how many attachment points are stored in this appendix.
            /// </summary>
            public ushort Count
            {
                get { return _count; }
                set { _count = value; }
            }

            /// <summary>
            /// Gets or sets attachment points.
            /// </summary>
            public List<AttachmentPoint> Points
            {
                get { return _points; }
                set { _points = value; }
            }

            /// <summary>
            /// Creates new instance.
            /// </summary>
            public AttachmentList()
            {
                _points = new List<AttachmentPoint>();
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

                _count = reader.ReadUInt16();

                for (int i = 0; i < _count; i++)
                {
                    AttachmentPoint point = new AttachmentPoint();
                    point.ReadFrom(reader);
                }
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

                writer.Write(_count);

                foreach (AttachmentPoint point in _points)
                    point.WriteTo(writer);
            }
            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public void Update()
            {
                _count = (ushort)_points.Count();
            }

            internal int GetSize()
            {
                return sizeof(ushort) + AttachmentPoint.SizeOf * _points.Count;
            }
        }

        /// <summary>
        /// Represents single attachment point.
        /// </summary>
        public class AttachmentPoint
        {
            internal const int SizeOf = sizeof(short) + sizeof(short);

            /// <summary>
            /// X position of attachment point.
            /// </summary>
            public short X;
            /// <summary>
            /// Y position of attachment point.
            /// </summary>
            public short Y;

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
            public void ReadFrom(BinaryReader reader)
            {
                if (reader == null)
                    throw new ArgumentNullException("reader");

                X = reader.ReadInt16();
                Y = reader.ReadInt16();
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

                writer.Write(X);
                writer.Write(Y);
            }
        }
    }
}
