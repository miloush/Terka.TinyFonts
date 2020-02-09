using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Font appendix for glyph metadata.
    /// </summary>
    public partial class GlyphMetadataAppendix : FontAppendix
    {
        /// <summary>
        /// ID of grapheme metadata.
        /// </summary>
        public const byte GraphemeSet = (byte)'g';
        /// <summary>
        /// ID of top margin metadata.
        /// </summary>
        public const byte MarginTopSet = (byte)'t';
        /// <summary>
        /// ID of bottom margin metadata.
        /// </summary>
        public const byte MarginBottomSet = (byte)'b';

        private SentinelCollection<MetadataSetOffset> _setsOffsets;
        private List<byte[]> _setsData;

        /// <summary>
        /// Gets metadata set collection in this appendix.
        /// </summary>
        public List<byte[]> Sets { get { return _setsData; } }
        /// <summary>
        /// Gets offsets of sets.
        /// </summary>
        public SentinelCollection<MetadataSetOffset> SetsOffsets { get { return _setsOffsets; } }

        /// <summary>
        /// Gets if appendix has any valid content.
        /// </summary>
        public override bool HasContent
        {
            get { return _setsData.Count > 0; }
        }

        /// <summary>
        /// Gets size of this appendix.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <returns>Size in bytes.</returns>
        public override int GetSize(TinyFont font)
        {
            return _setsOffsets.Count * MetadataSetOffset.SizeOf + _setsData.Sum(m => m.Length); 
        }

        /// <summary>
        /// Creates new instance of appendix.
        /// </summary>
        public GlyphMetadataAppendix()
            : base(GlyphMetadata)
        {
            _setsOffsets = new SentinelCollection<MetadataSetOffset>();
            _setsOffsets.Sentinel = new MetadataSetOffset();
            _setsOffsets.Sentinel.Id = 0xFF;

            _setsData = new List<byte[]>();
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public override void ReadFrom(BinaryReader reader, TinyFont font)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            while(true)
            {
                MetadataSetOffset offset = new MetadataSetOffset();
                offset.ReadFrom(reader);

                if (offset.Id == 0xFF)
                {
                    _setsOffsets.Sentinel = offset;
                    break;
                }

                _setsOffsets.Add(offset);
            }

            for (int i = 0; i < _setsOffsets.ItemsCount; i++)
            {
                byte[] data = reader.ReadBytes(_setsOffsets[i + 1].Offset - _setsOffsets[i].Offset);
                Sets.Add(data);
            }
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public override void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (MetadataSetOffset offset in _setsOffsets)
                offset.WriteTo(writer);

            foreach (byte[] data in _setsData)
                writer.Write(data);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            UpdateAssert(_setsData.Count <= ushort.MaxValue, "Too many metadata sets.");
            UpdateAssert(_setsData.Count == _setsOffsets.ItemsCount, "Metadata set offset count mismatch.");

            SortedDictionary<MetadataSetOffset, byte[]> sortedSets = new SortedDictionary<MetadataSetOffset, byte[]>();
            for (int i = 0; i < _setsOffsets.ItemsCount; i++)
                sortedSets.Add(_setsOffsets[i], _setsData[i]);

            _setsOffsets.Clear();
            _setsOffsets.AddRange(sortedSets.Keys);

            _setsData.Clear();
            _setsData.AddRange(sortedSets.Values);

            int offset = 0;
            for (int i = 0; i < _setsData.Count; i++)
            {
                UpdateAssert(offset <= ushort.MaxValue, "Too large metadata set.");

                _setsOffsets[i].Offset = (ushort)offset;
                offset += _setsData[i].Length;
            }

            _setsOffsets.Sentinel.Id = 0xFF;
            _setsOffsets.Sentinel.Offset = offset;
        }
        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }
    }
}
