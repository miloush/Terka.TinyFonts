namespace Win32
{
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The DWRITE_MATRIX structure specifies the graphics transform to be applied
        /// to rendered glyphs.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal partial struct Matrix
        {
            /// <summary>
            /// Horizontal scaling / cosine of rotation
            /// </summary>
            public float M11;

            /// <summary>
            /// Vertical shear / sine of rotation
            /// </summary>
            public float M12;

            /// <summary>
            /// Horizontal shear / negative sine of rotation
            /// </summary>
            public float M21;

            /// <summary>
            /// Vertical scaling / cosine of rotation
            /// </summary>
            public float M22;

            /// <summary>
            /// Horizontal shift (always orthogonal regardless of rotation)
            /// </summary>
            public float OffsetX;

            /// <summary>
            /// Vertical shift (always orthogonal regardless of rotation)
            /// </summary>
            public float OffsetY;

            public Matrix(float m11, float m12, float m21, float m22, float offsetX, float offsetY)
            {
                M11 = m11;
                M12 = m12;
                M21 = m21;
                M22 = m22;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
        };
    }
}
