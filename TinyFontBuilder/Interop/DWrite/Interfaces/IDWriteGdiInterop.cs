namespace Win32
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The GDI interop interface provides interoperability with GDI.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteGdiInterop), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDWriteGdiInterop
        {
            /// <summary>
            /// Creates a font object that matches the properties specified by the LOGFONT structure.
            /// </summary>
            /// <param name="logFont">Structure containing a GDI-compatible font description.</param>
            /// <returns>A newly created font object if successful, or NULL in case of error.</returns>
            IDWriteFont CreateFontFromLOGFONT(ref Gdi32.LogFont logFont);

            /// <summary>
            /// Initializes a LOGFONT structure based on the GDI-compatible properties of the specified font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_ConvertFontToLOGFONT();

            /// <summary>
            /// Initializes a LOGFONT structure based on the GDI-compatible properties of the specified font.
            /// </summary>
            /// <param name="font">Specifies a font face.</param>
            /// <returns>Structure that receives a GDI-compatible font description.</returns>
            Gdi32.LogFont ConvertFontFaceToLOGFONT(IDWriteFontFace font);

            /// <summary>
            /// Creates a font face object that corresponds to the currently selected HFONT.
            /// </summary>
            /// <param name="hDC">Handle to a device context into which a font has been selected. It is assumed that the client
            /// has already performed font mapping and that the font selected into the DC is the actual font that would be used 
            /// for rendering glyphs.</param>
            /// <returns>The newly created font face object, or NULL in case of failure.</returns>
            IDWriteFontFace CreateFontFaceFromHdc(IntPtr hDC);

            /// <summary>
            /// Creates an object that encapsulates a bitmap and memory DC which can be used for rendering glyphs.
            /// </summary>
            /// <param name="hDC">Optional device context used to create a compatible memory DC.</param>
            /// <param name="width">Width of the bitmap.</param>
            /// <param name="height">Height of the bitmap.</param>
            /// <returns>The newly created render target.</returns>
            IDWriteBitmapRenderTarget CreateBitmapRenderTarget(IntPtr hDC, uint width, uint height);
        }
    }
}
