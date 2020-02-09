namespace Terka.TinyFonts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    partial class GlyphClassesAppendix
    {
        /// <summary>
        /// Coverage information.
        /// </summary>
        public class Coverage
        {
            internal const int SizeOf = sizeof(ushort) + sizeof(ushort);

            private ushort _offset;
            private ushort _count;

            /// <summary>
            /// Gets or sets zero-based offset to <see cref="CoverageGlyphs"/> in <see cref="GlyphClassesAppendix"/>.
            /// </summary>
            public ushort Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }
            /// <summary>
            /// Gets or sets how many glyphs are assigned to this coverage.
            /// </summary>
            public ushort Count
            {
                get { return _count; }
                set { _count = value; }
            }

            /// <summary>
            /// Creates new coverage.
            /// </summary>
            public Coverage()
            {

            }
            internal Coverage(ushort offset, ushort count)
            {
                _offset = offset;
                _count = count;
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

                _offset = reader.ReadUInt16();
                _count = reader.ReadUInt16();
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

                writer.Write(_offset);
                writer.Write(_count);
            }

            /// <summary>
            /// Gets glyphs within this coverage.
            /// </summary>
            /// <param name="appendix"><see cref="GlyphClassesAppendix"/> containing this coverage.</param>
            /// <returns>Glyph's ID.</returns>
            public IEnumerable<int> GetGlyphs(GlyphClassesAppendix appendix)
            {
                if (_offset < 0 || _offset + _count > appendix.CoverageGlyphs.Count)
                    throw new InvalidOperationException("Out of range.");

                for (int i = 0; i < _count; i++)
                    yield return appendix.CoverageGlyphs[i + _offset];
            }
        }
    }
}
