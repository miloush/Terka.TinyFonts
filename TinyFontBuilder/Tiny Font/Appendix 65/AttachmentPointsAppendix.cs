using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Appendix for storing attachment points.
    /// </summary>
    public partial class AttachmentPointsAppendix : FontAppendix
    {
        private List<ushort> _offsets;
        private ushort _attachmentListSize;
        private List<AttachmentList> _lists;

        /// <summary>
        /// Gets size of this appendix in bytes.
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public override int GetSize(TinyFont font)
        {
            return sizeof(ushort) + sizeof(ushort) * _offsets.Count + _lists.Sum(l => l.GetSize());
        }

        /// <summary>
        /// Creates new instance of this appendix.
        /// </summary>
        public AttachmentPointsAppendix()
            : base(AttachmentPoints)
        {
            _offsets = new List<ushort>();
            _lists = new List<AttachmentList>();
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public override void ReadFrom(BinaryReader reader, TinyFont font)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _offsets.Clear();

            for (int i = 0; i < font.TotalCharactersCount; i++)
                _offsets.Add(reader.ReadUInt16());

            _attachmentListSize = reader.ReadUInt16();

            _lists.Clear();

            foreach (ushort offset in _offsets)
                if (offset > 0)
                {
                    AttachmentList list = new AttachmentList();
                    list.ReadFrom(reader);
                    _lists.Add(list);
                }
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public override void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (ushort offset in _offsets)
                writer.Write(offset);

            writer.Write(_attachmentListSize);

            foreach (AttachmentList list in _lists)
                list.WriteTo(writer);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            UpdateAssert(_lists.Count <= ushort.MaxValue, "Too many attachment points lists.");

            _offsets.Clear();
            int offset = 1;
            
            foreach (AttachmentList list in _lists)
            {
                list.Update();
                _offsets.Add((ushort)offset);

                offset += (list.GetSize() / sizeof(ushort));

                UpdateAssert(offset <= ushort.MaxValue, "Too many attachment points.");
            }

            _attachmentListSize = (ushort)(offset - 1);

        }
        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }
    }
}
