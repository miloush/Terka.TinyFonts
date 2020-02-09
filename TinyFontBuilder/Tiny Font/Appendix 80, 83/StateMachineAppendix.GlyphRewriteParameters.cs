namespace Terka.TinyFonts
{
    using System;
    using System.Collections.Generic;

    partial class StateMachineAppendix
    {
        /// <summary>
        /// Glyph rewriting heap parameters.
        /// </summary>
        public class GlyphRewriteParameters : HeapParameters
        {
            private ushort _rewriteCount;
            private ushort _writeCount;
            private List<int> _glyphs;

            /// <summary>
            /// Gets or sets how many glyphs should be rewrited.
            /// </summary>
            public ushort RewriteCount
            {
                get { return _rewriteCount; }
                set { _rewriteCount = value; }
            }
            /// <summary>
            /// Gets or sets how many glyphs should be written.
            /// </summary>
            public ushort WriteCount
            {
                get { return _writeCount; }
                set { _writeCount = value; }
            }
            /// <summary>
            /// Gets glyphs to be written.
            /// </summary>
            public IList<int> Glyphs
            {
                get { return _glyphs; }
            }

            /// <summary>
            /// Creates new instance of rewriting parameters.
            /// </summary>
            public GlyphRewriteParameters()
            {
                _glyphs = new List<int>();
            }
            /// <summary>
            /// Creates new instance of rewriting parameters.
            /// </summary>
            /// <param name="rewriteCount">How many glyphs will be rewrited.</param>
            /// <param name="glyphs">Which glyphs will be written.</param>
            public GlyphRewriteParameters(byte rewriteCount, params int[] glyphs)
            {
                _rewriteCount = rewriteCount;
                
                if (glyphs == null)
                    _glyphs = new List<int>();

                else
                {
                    _glyphs = new List<int>(glyphs);
                    _writeCount = (byte)_glyphs.Count;
                }
            }

            /// <summary>
            /// Reads parameters from byte array <paramref name="data"/> starting from zero-base <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Byte array containing heap.</param>
            /// <param name="offset">Zero-based offset to <paramref name="data"/> heap.</param>
            public override void ReadFrom(byte[] data, int offset)
            {
                _rewriteCount = data[offset++];
                _writeCount = data[offset++];

                for (int i = 0; i < _writeCount; i++, offset += sizeof(int))
                {
                    int glyph = (int)(data[offset] | data[offset + 1] << 8 | data[offset + 1] << 16 | data[offset + 1] << 24);
                    _glyphs.Add(glyph);
                }
            }
            /// <summary>
            /// Writes parameters to byte array <paramref name="data"/> starting on zero-based <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Heap data.</param>
            /// <param name="offset">Starting offset in <paramref name="data"/>.</param>
            public override void WriteTo(byte[] data, int offset)
            {
                data[offset++] = (byte)(_rewriteCount);
                data[offset++] = (byte)(_rewriteCount >> 8);
                data[offset++] = (byte)(_writeCount);
                data[offset++] = (byte)(_writeCount >> 8);

                if (_glyphs != null)
                    for (int i = 0; i < _glyphs.Count; i++)
                    {
                        int glyph = _glyphs[i];

                        data[offset++] = (byte)(glyph);
                        data[offset++] = (byte)(glyph >> 8);
                        data[offset++] = (byte)(glyph >> 16);
                        data[offset++] = (byte)(glyph >> 24);
                    }
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public override void Update()
            {
                UpdateAssert(_glyphs.Count <= byte.MaxValue, "Too many glyphs to write.");

                _writeCount = (byte)(_glyphs == null ? 0 : _glyphs.Count);
            }

            private static void UpdateAssert(bool condition, string error)
            {
                if (condition == false)
                    throw new InvalidOperationException(error);
            }

            /// <summary>
            /// Gets size in bytes of parameters.
            /// </summary>
            /// <returns>Size in bytes.</returns>
            public override int GetSize()
            {
                return sizeof(ushort) + sizeof(ushort) + (_glyphs == null ? 0 : sizeof(int) * _glyphs.Count);
            }
        }
    }
}
