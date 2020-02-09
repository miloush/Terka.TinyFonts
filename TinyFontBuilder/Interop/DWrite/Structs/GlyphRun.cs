namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The DWRITE_GLYPH_RUN structure contains the information needed by renderers
        /// to draw glyph runs. All coordinates are in device independent pixels (DIPs).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct GlyphRun : IDisposable
        {
            /// <summary>
            /// The physical font face to draw with.
            /// </summary>
            public IDWriteFontFace FontFace;
            
            /// <summary>
            /// Logical size of the font in DIPs, not points (equals 1/96 inch).
            /// </summary>
            public float EmSize;

            /// <summary>
            /// The number of glyphs.
            /// </summary>
            public uint GlyphCount;

            /// <summary>
            /// The indices to render.
            /// </summary>    
            public IntPtr GlyphIndices;

            /// <summary>
            /// Glyph advance widths.
            /// </summary>
            public IntPtr GlyphAdvances;

            /// <summary>
            /// Glyph offsets.
            /// </summary>
            public IntPtr GlyphOffsets;

            /// <summary>
            /// If true, specifies that glyphs are rotated 90 degrees to the left and
            /// vertical metrics are used. Vertical writing is achieved by specifying
            /// isSideways = true and rotating the entire run 90 degrees to the right
            /// via a rotate transform.
            /// </summary>
            public bool IsSideways;

            /// <summary>
            /// The implicit resolved bidi level of the run. Odd levels indicate
            /// right-to-left languages like Hebrew and Arabic, while even levels
            /// indicate left-to-right languages like English and Japanese (when
            /// written horizontally). For right-to-left languages, the text origin
            /// is on the right, and text should be drawn to the left.
            /// </summary>
            public uint BidiLevel;

            internal GlyphRun(ushort singleGlyph) : this()
            {
                GlyphCount = 1;

                GlyphIndices = Marshal.AllocHGlobal(sizeof(ushort));
                Marshal.WriteInt16(GlyphIndices, (short)singleGlyph);

                GlyphAdvances = Marshal.AllocHGlobal(sizeof(float));
                Marshal.WriteInt32(GlyphAdvances, 0);

                GlyphOffsets = Marshal.AllocHGlobal(sizeof(float) + sizeof(float));
                Marshal.WriteInt64(GlyphOffsets, 0);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(GlyphIndices);
                Marshal.FreeHGlobal(GlyphAdvances);
                Marshal.FreeHGlobal(GlyphOffsets);
            }
        }
    }
}
