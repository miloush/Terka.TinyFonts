namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// Encapsulates a 32-bit device independent bitmap and device context, which can be used for rendering glyphs.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteBitmapRenderTarget1), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDWriteBitmapRenderTarget1
        {
            /// <summary>
            /// Draws a run of glyphs to the bitmap.
            /// </summary>
            /// <param name="baselineOriginX">Horizontal position of the baseline origin, in DIPs, relative to the upper-left corner of the DIB.</param>
            /// <param name="baselineOriginY">Vertical position of the baseline origin, in DIPs, relative to the upper-left corner of the DIB.</param>
            /// <param name="measuringMode">Specifies measuring mode for glyphs in the run.
            /// Renderer implementations may choose different rendering modes for different measuring modes, for example
            /// DWRITE_RENDERING_MODE_CLEARTYPE_NATURAL for DWRITE_MEASURING_MODE_NATURAL,
            /// DWRITE_RENDERING_MODE_CLEARTYPE_GDI_CLASSIC for DWRITE_MEASURING_MODE_GDI_CLASSIC, and
            /// DWRITE_RENDERING_MODE_CLEARTYPE_GDI_NATURAL for DWRITE_MEASURING_MODE_GDI_NATURAL.
            /// </param>
            /// <param name="glyphRun">Structure containing the properties of the glyph run.</param>
            /// <param name="renderingParams">Object that controls rendering behavior.</param>
            /// <param name="textColor">Specifies the foreground color of the text.</param>
            /// <returns>Rectangle that receives the bounding box (in pixels not DIPs) of all the pixels affected by 
            /// drawing the glyph run. The black box rectangle may extend beyond the dimensions of the bitmap.
            /// </returns>
            Rect DrawGlyphRun(float baselineOriginX, float baselineOriginY, MeasuringMode measuringMode, ref GlyphRun glyphRun, IDWriteRenderingParams renderingParams, uint textColor);

            /// <summary>
            /// Gets a handle to the memory device context.
            /// </summary>
            /// <returns>The device context handle.</returns>
            /// <remarks>
            /// An application can use the device context to draw using GDI functions. An application can obtain the bitmap handle
            /// (HBITMAP) by calling GetCurrentObject. An application that wants information about the underlying bitmap, including
            /// a pointer to the pixel data, can call GetObject to fill in a DIBSECTION structure. The bitmap is always a 32-bit 
            /// top-down DIB.
            /// </remarks>
            [PreserveSig]
            IntPtr GetMemoryDC();

            /// <summary>
            /// Gets the number of bitmap pixels per DIP. A DIP (device-independent pixel) is 1/96 inch so this value is the number
            /// if pixels per inch divided by 96.
            /// </summary>
            /// <returns>The number of bitmap pixels per DIP.</returns>
            [PreserveSig]
            float GetPixelsPerDip();

            /// <summary>
            /// Sets the number of bitmap pixels per DIP. A DIP (device-independent pixel) is 1/96 inch so this value is the number
            /// if pixels per inch divided by 96.
            /// </summary>
            /// <param name="pixelsPerDip">Specifies the number of pixels per DIP.</param>
            void SetPixelsPerDip(float pixelsPerDip);

            /// <summary>
            /// Gets the transform that maps abstract coordinate to DIPs. By default this is the identity 
            /// transform. Note that this is unrelated to the world transform of the underlying device
            /// context.
            /// </summary>
            /// <returns>The transform.</returns>
            Matrix GetCurrentTransform();

            /// <summary>
            /// Sets the transform that maps abstract coordinate to DIPs. This does not affect the world
            /// transform of the underlying device context.
            /// </summary>
            /// <param name="transform">Specifies the new transform. This parameter can be NULL, in which
            /// case the identity transform is implied.</param>
            void SetCurrentTransform(ref Matrix transform);

            /// <summary>
            /// Gets the dimensions of the bitmap.
            /// </summary>
            /// <returns>The size of the bitmap in pixels.</returns>
            Size GetSize();

            /// <summary>
            /// Resizes the bitmap.
            /// </summary>
            /// <param name="width">New bitmap width, in pixels.</param>
            /// <param name="height">New bitmap height, in pixels.</param>
            void Resize(int width, int height);


            /// <summary>
            /// Gets the current text antialiasing mode of the bitmap render target.
            /// </summary>
            /// <returns>The antialiasing mode.</returns>
            [PreserveSig]
            TextAntialiasMode GetTextAntialiasMode();

            /// <summary>
            /// Sets the current text antialiasing mode of the bitmap render target.
            /// </summary>
            /// <param name="mode">The antialiasing mode.</param>
            /// <remarks>
            /// The antialiasing mode of a newly-created bitmap render target defaults to 
            /// DWRITE_TEXT_ANTIALIAS_MODE_CLEARTYPE. An application can change the antialiasing
            /// mode by calling SetTextAntialiasMode. For example, an application might specify
            /// grayscale antialiasing when rendering text onto a transparent bitmap.
            /// </remarks>
            void SetTextAntialiasMode(TextAntialiasMode mode);
        }
    }
}
