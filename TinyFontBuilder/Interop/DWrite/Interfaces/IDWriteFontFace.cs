namespace Win32
{
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The interface that represents an absolute reference to a font face.
        /// It contains font face type, appropriate file references and face identification data.
        /// Various font data such as metrics, names and glyph outlines is obtained from IDWriteFontFace.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteFontFace), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDWriteFontFace
        {

        }
    }
}
