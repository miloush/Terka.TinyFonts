using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terka.TinyBitmaps;
using Terka.TinyResources;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Represents a Tiny Font.
    /// </summary>
    public partial class TinyFont
    {
        private const int LastPlane = 16;

        private FontPlane _baseFontPlane;
        private FontPlanesCollection _fontPlanes;
        private List<FontAppendix> _appendices;

        /// <summary>
        /// Gets or sets font metrics.
        /// </summary>
        public FontMetrics Metrics
        {
            get { return _baseFontPlane.Metrics; }
            set { _baseFontPlane.Metrics = value; }
        }
        /// <summary>
        /// Gets or sets font description.
        /// </summary>
        public FontDescription Description
        {
            get { return _baseFontPlane.Description; }
            set { _baseFontPlane.Description = value; }
        }
        /// <summary>
        /// Gets or sets character bitmap.
        /// </summary>
        public BitmapDescription CharacterBitmap
        {
            get { return _baseFontPlane.CharacterBitmap; }
            set { _baseFontPlane.CharacterBitmap = value; }
        }
        /// <summary>
        /// Gets or sets character ranges. The sequence of character ranges is ended by a sentinel character range, which has all fields set to zero except the range offset, which is set to the bitmap’s width.
        /// </summary>
        public SentinelCollection<CharacterRangeDescription> CharacterRanges
        {
            get { return _baseFontPlane.CharacterRanges; }
        }
        /// <summary>
        /// Gets or sets characters of this plane. The sequence is ended by a sentinel character, which has all fields set to zero.
        /// </summary>
        public SentinelCollection<CharacterDescription> Characters
        {
            get { return _baseFontPlane.Characters; }
        }
        /// <summary>
        /// Gets or sets characters raw bitmap data. 
        /// The image is stored top to bottom by rows, left to right in bits from the least significant bit (LSB) to the most significant bit (MSB).
        /// All included characters are stacked on a single row, side by side to the right in the order of range and character sequences.
        /// </summary>
        public byte[] CharacterBitmapData
        {
            get { return _baseFontPlane.CharacterBitmapData; }
            set { _baseFontPlane.CharacterBitmapData = value; }
        }

        /// <summary>
        /// Gets or sets anti-aliasing metrics for characters.
        /// </summary>
        public AntialiasingMetrics CharacterAntialiasingMetrics
        {
            get { return _baseFontPlane.CharacterAntialiasingMetrics; }
            set { _baseFontPlane.CharacterAntialiasingMetrics = value; }
        }
        /// <summary>
        /// Gets or sets character range anti-aliasing offsets.
        /// </summary>
        public IList<CharacterRangeAntialiasing> CharacterRangesAntialiasing
        {
            get { return _baseFontPlane.CharacterRangesAntialiasing; }
        }
        /// <summary>
        /// Gets or sets character anti-aliasing offsets.
        /// </summary>
        public IList<CharacterAntialiasing> CharactersAntialiasing
        {
            get { return _baseFontPlane.CharactersAntialiasing; }
        }
        /// <summary>
        /// Gets or sets the anti-aliasing data for Tiny Font.
        /// Data are raw values without any alignment. The total size of anti-alias data, however, is aligned at 4 byte boundary, so the number of bytes is equal to antialias size plus eventual padding.
        /// </summary>
        public byte[] CharacterAntialiasingData
        {
            get { return _baseFontPlane.CharacterAntialiasingData; }
            set { _baseFontPlane.CharacterAntialiasingData = value; }
        }

        /// <summary>
        /// Gets font appendicies.
        /// </summary>
        public IList<FontAppendix> Appendices
        {
            get { return _appendices; }
        }
        /// <summary>
        /// Gets font planes collection.
        /// </summary>
        public FontPlanesCollection FontPlanes
        {
            get { return _fontPlanes; }
        }

        /// <summary>
        /// Gets the total count of characters stored in this tiny font file.
        /// </summary>
        public int TotalCharactersCount
        {
            get
            {
                int count = _baseFontPlane.Characters.ItemsCount;

                UnicodePlanesAppendix planesAppendix;
                if (TryGetAppendix(out planesAppendix))
                    foreach (FontPlane plane in planesAppendix.Planes)
                        count += plane.Characters.ItemsCount;

                return count;
            }
        }

        /// <summary>
        /// Gets existing appendix and if appendix of this type does not exists yet, creates new empty one.
        /// </summary>
        /// <typeparam name="T">Type of requested font appendix.</typeparam>
        /// <returns>Requested font appendix.</returns>
        public T GetOrAddNewAppendix<T>() where T : FontAppendix, new()
        {
            T appendix = _appendices.OfType<T>().FirstOrDefault();

            if (appendix == null)
            {
                appendix = new T();
                _appendices.Add(appendix);
            }

            return appendix;
        }

        /// <summary>
        /// Tries to get font appendix in this tiny font file.
        /// </summary>
        /// <typeparam name="T">Type of requested font appendix.</typeparam>
        /// <param name="appendix">Found appendix or null if appendix does not exists.</param>
        /// <returns>True if appendix was found.</returns>
        public bool TryGetAppendix<T>(out T appendix) where T : FontAppendix
        {
            appendix = _appendices.OfType<T>().FirstOrDefault();
            return appendix != null;
        }

        /// <summary>
        /// Creates new empty instance of Tiny Font.
        /// </summary>
        public TinyFont()
        {
            _baseFontPlane = new FontPlane();
            _fontPlanes = new FontPlanesCollection(this);
            _appendices = new List<FontAppendix>();
        }
        /// <summary>
        /// Creates new instance of Tiny Font from resources.
        /// </summary>
        /// <param name="resource">Resource containing seralized data from tinyfnt file.</param>
        public TinyFont(TinyResourcesFile.Resource resource)
            : this()
        {
            if (resource == null)
                throw new ArgumentNullException();
            if (resource.Header.Kind != TinyResourceKind.Font)
                throw new ArgumentException();
            if (resource.Data == null)
                throw new ArgumentException();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(resource.Data)))
                ReadFrom(reader);
        }

        private TinyFont(string path)
            : this()
        {
            if (path == null)
                throw new ArgumentNullException();

            TinyResourcesFile resources = new TinyResourcesFile();
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path), Encoding.UTF8))
                resources.ReadFrom(reader);

            TinyResourcesFile.Resource fontResource = null;
            try { fontResource = resources.Single(resource => resource.Header.Kind == TinyResourceKind.Font); }
            catch (InvalidOperationException e) { throw new ArgumentException("The resource file does not contain any font resources.", e); }

            using (BinaryReader reader = new BinaryReader(new MemoryStream(fontResource.Data)))
                ReadFrom(reader);
        }

        /// <summary>
        /// Creates new instance of Tiny Font from tinyfnt file from requested path.
        /// </summary>
        /// <param name="path">Path to tinyfnt file.</param>
        /// <returns>Corresponding Tiny Font structure.</returns>
        public static TinyFont Load(string path)
        {
            return new TinyFont(path);
        }
        /// <summary>
        /// Creates new instance of Tiny Font from binary reader.
        /// </summary>
        /// <param name="reader">Source or serialized data.</param>
        /// <returns>New instance of Tiny Font.</returns>
        public static TinyFont Load(BinaryReader reader)
        {
            TinyFont font = new TinyFont();
            font.ReadFrom(reader);
            return font;
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

            _baseFontPlane.ReadFrom(reader, this);

            ReadAppendicesFrom(reader);
        }

        private void ReadAppendicesFrom(BinaryReader reader)
        {
            _appendices.Clear();

            for (int i = 0; i < Description.Appendices; i++)
            {
                int header = reader.ReadInt32();

                byte id = (byte)header;
                int size = header >> 8;

                FontAppendix appendix = CreateAppendix(id, size);
                appendix.ReadFrom(reader, this);
                _appendices.Add(appendix);

                int padding = 4 - (int)size % 4;
                if (padding < 4)
                    reader.ReadBytes(padding);
            }
        }

        private FontAppendix CreateAppendix(byte id, int size)
        {
            switch (id)
            {
                case FontAppendix.UnicodePlane:
                    return new UnicodePlanesAppendix();

                case FontAppendix.PositioningMachine:
                    return new PositioningAppendix();

                case FontAppendix.GlyphClasses:
                    return new GlyphClassesAppendix();

                case FontAppendix.SubstitutionMachine:
                    return new SubstitutionAppendix();

                case FontAppendix.GlyphMetadata:
                    return new GlyphMetadataAppendix();

                default:
                    return new UnknownAppendix(id, size);
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
                throw new ArgumentNullException();

            _baseFontPlane.WriteTo(writer, this);

            WriteAppendicesTo(writer);
        }

        private void WriteAppendicesTo(BinaryWriter writer)
        {
            for (int i = 0; i < Description.Appendices; i++)
            {
                int header = _appendices[i].ID + (_appendices[i].GetSize(this) << 8);

                writer.Write(header);
                _appendices[i].WriteTo(writer, this);
                
                int padding = 4 - (int)_appendices[i].GetSize(this) % 4;
                if (padding < 4)
                    writer.Write(new byte[padding]);
            }
        }

        /// <summary>
        /// Saves serialized Tiny Font to stream.
        /// </summary>
        /// <param name="stream">Stream to save serialized data.</param>
        public void Save(Stream stream)
        {
            Update();

            MemoryStream serialized = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(serialized, Encoding.UTF8))
                WriteTo(writer);

            TinyResourcesFile resources = new TinyResourcesFile();
            resources.Add(new TinyResourcesFile.Resource(TinyResourceKind.Font, serialized.ToArray()));
            resources.Save(stream);
        }
        /// <summary>
        /// Saves serialized Tiny Font to file.
        /// </summary>
        /// <param name="path">Path to file to save serialized data.</param>
        public void Save(string path)
        {
            using (Stream stream = File.Create(path))
                Save(stream);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        public void Update()
        {
            _baseFontPlane.Update();

            UpdateAssert(_appendices.Count <= ushort.MaxValue, "Too many appendices.");

            foreach (FontAppendix appendix in _appendices)
                appendix.Update(this);

            _appendices.RemoveAll((FontAppendix a) => !a.HasContent);
            Description.Appendices = (ushort)_appendices.Count;

            UpdatePlanes();
        }
        private void UpdatePlanes()
        {
            long height = _baseFontPlane.CharacterBitmap.Height + _baseFontPlane.Metrics.Offset;

            foreach (UnicodePlanesAppendix appendix in Appendices.OfType<UnicodePlanesAppendix>())
                foreach (FontPlane plane in appendix.Planes)
                    height = Math.Max(height, plane.CharacterBitmap.Height + plane.Metrics.Offset);

            foreach (UnicodePlanesAppendix appendix in Appendices.OfType<UnicodePlanesAppendix>())
                foreach (FontPlane plane in appendix.Planes)
                    plane.Metrics.Offset = (short)(height - plane.CharacterBitmap.Height);

            _baseFontPlane.Metrics.Offset = (short)(height - _baseFontPlane.CharacterBitmap.Height);
        }

        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }

        /// <summary>
        /// Returns information for character specified by its code.
        /// </summary>
        /// <param name="c">Character code.</param>
        /// <returns>Character information.</returns>
        public CharacterInfo GetCharacterInfo(int c)
        {
            int planeNumber = c >> 16;

            FontPlane plane = FontPlanes[planeNumber];

            if (plane != null)
                return GetCharacterInfo((char)c, plane, planeNumber);

            return null;
        }
        /// <summary>
        /// Returns information for character specified by char.
        /// </summary>
        /// <param name="c">Character.</param>
        /// <returns>Character information.</returns>
        public CharacterInfo GetCharacterInfo(char c)
        {
            return GetCharacterInfo(c, _baseFontPlane, 0);
        }
        private CharacterInfo GetCharacterInfo(char c, FontPlane plane, int planeNumber)
        {
            int rangeIndex = plane.CharacterRanges.BinarySearch(c, CharacterRangeComparer.Instance);

            if (rangeIndex >= 0)
                return new CharacterInfo(this, rangeIndex, c, plane, planeNumber);

            return null;
        }
        /// <summary>
        /// Enumerates thru all characters saved in Tiny Font accross all font planes sorted by font plane in ascending order.
        /// </summary>
        /// <returns>Character information.</returns>
        public IEnumerable<CharacterInfo> EnumerateAllCharacterInfos()
        {
            for (int planeNumber = 0; planeNumber <= LastPlane; planeNumber++)
            {
                FontPlane plane = FontPlanes[planeNumber];
                if (plane != null)
                {
                    for (int r = 0; r < plane.CharacterRanges.ItemsCount; r++)
                    {
                        CharacterRangeDescription range = plane.CharacterRanges[r];
                        for (char c = range.FirstCharacter; c <= range.LastCharacter; c++)
                            yield return new CharacterInfo(this, r, c, plane, planeNumber);
                    }
                }
            }
        }
    }
}

