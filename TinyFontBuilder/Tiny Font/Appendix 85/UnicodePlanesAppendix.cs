using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terka.TinyBitmaps;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Appendix contaning extended Unicode planes.
    /// </summary>
    public partial class UnicodePlanesAppendix : FontAppendix
    {
        private ushort _mask;
        private ushort _reserved;
        private List<FontPlane> _fontPlanes;

        /// <summary>
        /// Gets bit mask representing which font planes are present in this appendix.
        /// </summary>
        public ushort Mask { get { return _mask; } }
        /// <summary>
        /// Gets list of planes in this appendix.
        /// </summary>
        public List<FontPlane> Planes { get { return _fontPlanes; } }

        /// <summary>
        /// Gets total size in bytes of this appendix.
        /// </summary>
        public override int GetSize(TinyFont font)
        {
            return sizeof(ushort) + sizeof(ushort) + _fontPlanes.Sum(plane => plane.GetSize(font));
        }

        /// <summary>
        /// Gets if this appendix has any valid content.
        /// </summary>
        public override bool HasContent
        {
            get { return _mask != 0; }
        }

        /// <summary>
        /// Creates new instance of this appendix.
        /// </summary>
        public UnicodePlanesAppendix()
            : base(UnicodePlane)
        {
            _fontPlanes = new List<FontPlane>();
        }

        /// <summary>
        /// Sets Font Plane data for specified plane <paramref name="number"/>.
        /// If plane does not exists yet, it will be appended otherwise existing updated.
        /// It also update bit mask.
        /// </summary>
        /// <param name="number">Plane number.</param>
        /// <param name="plane">Font Plane data.</param>
        public void SetPlane(int number, FontPlane plane)
        {
            int index = IndexOfPlane(number);

            if (index >= 0)
                _fontPlanes[index] = plane;
            else
                _fontPlanes.Insert(~index, plane);

            _mask |= (ushort)(1 << (number - 1));
        }

        /// <summary>
        /// Returns zero-based index to list of font planes <see cref="Planes"/> to specified plane <paramref name="number"/>.
        /// </summary>
        /// <param name="number">Plane number.</param>
        /// <returns>Zero-based index.</returns>
        public int IndexOfPlane(int number)
        {
            if (number < 1)
                throw new ArgumentOutOfRangeException("number");

            number--;

            int index = 0;

            for (int i = 0; i < number; i++)
            {
                if ((_mask & (1 << i)) != 0)
                {
                    index++;
                }
            }

            if ((_mask & (1 << number)) != 0)
            {
                return index;
            }

            return ~index;
        }

        /// <summary>
        /// Gets font plane by its plane <paramref name="number"/>.
        /// </summary>
        /// <param name="number">Plane number</param>
        /// <returns>Font Plane or null if plane with specified number does not exists.</returns>
        public FontPlane GetPlane(int number)
        {
            int index = IndexOfPlane(number);

            if (index < 0)
                return null;

            if (index >= _fontPlanes.Count)
                throw new InvalidOperationException("Planes and mask mismatch.");

            return _fontPlanes[index];
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="font"/> is null.</exception>
        public override void ReadFrom(BinaryReader reader, TinyFont font)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (font == null)
                throw new ArgumentNullException("font");

            _mask = reader.ReadUInt16();
            _reserved = reader.ReadUInt16();

            _fontPlanes.Clear();

            ushort positionMask = _mask;

            while (positionMask != 0)
            {
                if ((positionMask & 1) != 0)
                {
                    FontPlane plane = new FontPlane();
                    plane.ReadFrom(reader, font);

                    _fontPlanes.Add(plane);
                }

                positionMask >>= 1;
            }
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="font"/> is null.</exception>
        public override void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (font == null)
                throw new ArgumentNullException("font");

            writer.Write(_mask);
            writer.Write(_reserved);

            foreach (FontPlane plane in _fontPlanes)
                plane.WriteTo(writer, font);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            ushort positionMask = _mask;
            int removedMask = 0;

            int i = 0;
            while (positionMask != 0)
            {
                if ((positionMask & 1) != 0)
                {
                    UpdateAssert(i < _fontPlanes.Count, "Too few planes.");
                    UpdateAssert(_fontPlanes[i] != null, "Plane not initialized.");

                    _fontPlanes[i].Update();

                    if (!_fontPlanes[i].HasContent)
                    {
                        removedMask |= 1 << i;

                        _fontPlanes.RemoveAt(i);
                    }
                    else
                    {
                        // we need to keep all metrics in sync, but bitmap ~ offset can be different
                        short planeOffset = _fontPlanes[i].Metrics.Offset;
                        _fontPlanes[i].Metrics = (FontMetrics)font.Metrics.Clone();
                        _fontPlanes[i].Metrics.Offset = planeOffset;

                        i++;
                    }
                }

                positionMask >>= 1;
            }

            _mask &= (ushort)~removedMask;
        }
        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }
    }
}
