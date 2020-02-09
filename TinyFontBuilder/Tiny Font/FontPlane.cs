using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terka.TinyBitmaps;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Describes font plane (same for basic and extended planes).
    /// </summary>
    public class FontPlane
    {
        private FontMetrics _metrics;
        private FontDescription _description;
        private BitmapDescription _characterBitmapDescription;
        private SentinelCollection<CharacterRangeDescription> _characterRanges;
        private SentinelCollection<CharacterDescription> _characters;
        private byte[] _characterBitmapData;

        private AntialiasingMetrics _characterAntialiasingMetrics;
        private List<CharacterRangeAntialiasing> _characterRangesAntialiasing;
        private List<CharacterAntialiasing> _charactersAntialiasing;
        private byte[] _characterAntialiasingData;

        /// <summary>
        /// Gets or sets font metrics.
        /// </summary>
        public FontMetrics Metrics
        {
            get { return _metrics; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _metrics = value;
            }
        }
        /// <summary>
        /// Gets or sets font description.
        /// </summary>
        public FontDescription Description
        {
            get { return _description; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _description = value;
            }
        }
        /// <summary>
        /// Gets or sets character bitmap.
        /// </summary>
        public BitmapDescription CharacterBitmap
        {
            get { return _characterBitmapDescription; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _characterBitmapDescription = value;
            }
        }
        /// <summary>
        /// Gets or sets character ranges. The sequence of character ranges is ended by a sentinel character range, which has all fields set to zero except the range offset, which is set to the bitmap’s width.
        /// </summary>
        public SentinelCollection<CharacterRangeDescription> CharacterRanges
        {
            get { return _characterRanges; }
        }
        /// <summary>
        /// Gets or sets characters of this plane. The sequence is ended by a sentinel character, which has all fields set to zero.
        /// </summary>
        public SentinelCollection<CharacterDescription> Characters
        {
            get { return _characters; }
        }
        /// <summary>
        /// Gets or sets characters raw bitmap data. 
        /// The image is stored top to bottom by rows, left to right in bits from the least significant bit (LSB) to the most significant bit (MSB).
        /// All included characters are stacked on a single row, side by side to the right in the order of range and character sequences.
        /// </summary>
        public byte[] CharacterBitmapData
        {
            get { return _characterBitmapData; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _characterBitmapData = value;
            }
        }

        /// <summary>
        /// Gets or sets anti-aliasing metrics for characters.
        /// </summary>
        public AntialiasingMetrics CharacterAntialiasingMetrics
        {
            get { return _characterAntialiasingMetrics; }
            set
            {
                if (value == null)
                    value = new AntialiasingMetrics { Size = 0 };

                _characterAntialiasingMetrics = value;
            }
        }
        /// <summary>
        /// Gets or sets character range anti-aliasing offsets.
        /// </summary>
        public IList<CharacterRangeAntialiasing> CharacterRangesAntialiasing
        {
            get { return _characterRangesAntialiasing; }
        }
        /// <summary>
        /// Gets or sets character anti-aliasing offsets.
        /// </summary>
        public IList<CharacterAntialiasing> CharactersAntialiasing
        {
            get { return _charactersAntialiasing; }
        }
        /// <summary>
        /// Gets or sets the anti-aliasing data for Tiny Font.
        /// Data are raw values without any alignment. The total size of anti-alias data, however, is aligned at 4 byte boundary, so the number of bytes is equal to antialias size plus eventual padding.
        /// </summary>
        public byte[] CharacterAntialiasingData
        {
            get { return _characterAntialiasingData; }
            set { _characterAntialiasingData = value; }
        }

        /// <summary>
        /// Gets if this font plane has any valid content.
        /// </summary>
        public bool HasContent
        {
            get { return _characters.Count > 0; }
        }

        /// <summary>
        /// Creates new font plane.
        /// </summary>
        public FontPlane()
        {
            _metrics = new FontMetrics();
            _description = new FontDescription();
            _characterBitmapDescription = new BitmapDescription();
            _characterRanges = new SentinelCollection<CharacterRangeDescription>();
            _characters = new SentinelCollection<CharacterDescription>();
            _characterBitmapData = new byte[0];

            _characterAntialiasingMetrics = new AntialiasingMetrics();
            _characterRangesAntialiasing = new List<CharacterRangeAntialiasing>();
            _charactersAntialiasing = new List<CharacterAntialiasing>();
            _characterAntialiasingData = null;
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public void ReadFrom(BinaryReader reader, TinyFont font)
        {
            if (reader == null)
                throw new ArgumentNullException();

            if (font == null)
                throw new ArgumentNullException();

            _metrics.ReadFrom(reader);
            _description.ReadFrom(reader);
            _characterBitmapDescription.ReadFrom(reader);

            ReadBasicFrom(reader);

            if (font.Description.IsExtended)
                ReadExtendedFrom(reader);
        }
        private void ReadBasicFrom(BinaryReader reader)
        {
            _characterRanges.Clear();
            _characterRanges.Capacity = _description.Ranges;
            for (int i = 0; i < _description.Ranges; i++)
            {
                CharacterRangeDescription range = new CharacterRangeDescription();
                range.ReadFrom(reader);

                _characterRanges.Add(range);
            }
            _characterRanges.Sentinel = new CharacterRangeDescription();
            _characterRanges.Sentinel.ReadFrom(reader);

            _characters.Clear();
            _characters.Capacity = _description.Characters;
            for (int i = 0; i < _description.Characters; i++)
            {
                CharacterDescription character = new CharacterDescription();
                character.ReadFrom(reader);

                _characters.Add(character);
            }
            _characters.Sentinel = new CharacterDescription();
            _characters.Sentinel.ReadFrom(reader);

            int wordwidth = (int)((_characterBitmapDescription.Width * _characterBitmapDescription.BitsPerPixel + 31) / 32 * _characterBitmapDescription.Height * 4);
            _characterBitmapData = reader.ReadBytes(wordwidth);
        }
        private void ReadExtendedFrom(BinaryReader reader)
        {
            _characterAntialiasingMetrics.ReadFrom(reader);

            _characterRangesAntialiasing.Clear();
            _characterRangesAntialiasing.Capacity = _description.Ranges;
            for (int i = 0; i < _description.Ranges; i++)
            {
                CharacterRangeAntialiasing range = new CharacterRangeAntialiasing();
                range.ReadFrom(reader);

                _characterRangesAntialiasing.Add(range);
            }

            _charactersAntialiasing.Clear();
            _charactersAntialiasing.Capacity = _description.Characters;
            for (int i = 0; i < _description.Characters; i++)
            {
                CharacterAntialiasing character = new CharacterAntialiasing();
                character.ReadFrom(reader);

                _charactersAntialiasing.Add(character);
            }

            if (_description.Characters % 2 == 1)
                reader.ReadBytes(2);

            _characterAntialiasingData = reader.ReadBytes((int)_characterAntialiasingMetrics.Size);

            int padding = 4 - (int)_characterAntialiasingMetrics.Size % 4;
            if (padding < 4)
                reader.ReadBytes(padding);
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">The <see cref="TinyFont"/> containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (writer == null)
                throw new ArgumentNullException();

            if (font == null)
                throw new ArgumentNullException();

            _metrics.WriteTo(writer);
            _description.WriteTo(writer);
            _characterBitmapDescription.WriteTo(writer);

            WriteBasicTo(writer);

            if (font.Description.IsExtended)
                WriteExtendedTo(writer);
        }
        private void WriteBasicTo(BinaryWriter writer)
        {
            for (int i = 0; i < _characterRanges.Count; i++)
                _characterRanges[i].WriteTo(writer);

            for (int i = 0; i < _characters.Count; i++)
                _characters[i].WriteTo(writer);

            writer.Write(_characterBitmapData);
        }
        private void WriteExtendedTo(BinaryWriter writer)
        {
            _characterAntialiasingMetrics.WriteTo(writer);

            for (int i = 0; i < _characterRangesAntialiasing.Count; i++)
                _characterRangesAntialiasing[i].WriteTo(writer);

            for (int i = 0; i < _charactersAntialiasing.Count; i++)
                _charactersAntialiasing[i].WriteTo(writer);

            if (_charactersAntialiasing.Count % 2 == 1)
                writer.Write(new byte[2]);

            if (_characterAntialiasingData != null)
            {
                writer.Write(_characterAntialiasingData);

                int padding = 4 - (int)_characterAntialiasingData.Length % 4;
                if (padding < 4)
                    writer.Write(new byte[padding]);
            }
        }
        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        public void Update()
        {
            UpdateAssert(_characterRanges.Count <= ushort.MaxValue, "Too many character ranges.");
            UpdateAssert(_characters.Count <= ushort.MaxValue, "Too many characters.");

            _description.Ranges = (ushort)_characterRanges.ItemsCount;
            _description.Characters = (ushort)_characters.ItemsCount;

            if (_characterRanges.Sentinel == null)
                _characterRanges.Sentinel = new CharacterRangeDescription { Offset = _characterBitmapDescription.Width };

            if (_characters.Sentinel == null)
                _characters.Sentinel = new CharacterDescription();

            if (_characterAntialiasingData == null || _characterAntialiasingData.Length == 0)
            {
                _characterAntialiasingMetrics.Size = 0;
            }
            else
            {
                _characterAntialiasingMetrics.Size = (uint)_characterAntialiasingData.Length;
            }
        }
        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }

        internal int GetSize(TinyFont font)
        {
            return FontMetrics.SizeOf +
                   FontDescription.SizeOf +
                   BitmapDescription.SizeOf +
                   _characterRanges.Count * CharacterRangeDescription.SizeOf +
                   _characters.Count * CharacterDescription.SizeOf +
                   _characterBitmapData.Length +
                   (font.Description.IsExtended ? AntialiasingMetrics.SizeOf : 0) +
                   _characterRangesAntialiasing.Count * CharacterRangeAntialiasing.SizeOf +
                   _charactersAntialiasing.Count * CharacterAntialiasing.SizeOf +
                   (_characterAntialiasingData == null ? 0 : _characterAntialiasingData.Length);
        }
    }
}
