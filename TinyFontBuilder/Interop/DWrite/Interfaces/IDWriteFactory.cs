namespace Win32
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Text;
    using ComTypes = System.Runtime.InteropServices.ComTypes;

    partial class DWrite
    {
        /// <summary>
        /// The root factory interface for all DWrite objects.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteFactory), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDWriteFactory
        {
            /// <summary>
            /// Gets a font collection representing the set of installed fonts.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetSystemFontCollection();

            /// <summary>
            /// Creates a font collection using a custom font collection loader.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateCustomFontCollection();

            /// <summary>
            /// Registers a custom font collection loader with the factory object.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_RegisterFontCollectionLoader();

            /// <summary>
            /// Unregisters a custom font collection loader that was previously registered using RegisterFontCollectionLoader.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_UnregisterFontCollectionLoader();

            /// <summary>
            /// CreateFontFileReference creates a font file reference object from a local font file.
            /// </summary>
            /// <param name="filePath">Absolute file path. Subsequent operations on the constructed object may fail
            /// if the user provided filePath doesn't correspond to a valid file on the disk.</param>
            /// <param name="lastWriteTime">Last modified time of the input file path. If the parameter is omitted,
            /// the function will access the font file to obtain its last write time, so the clients are encouraged to specify this value
            /// to avoid extra disk access. Subsequent operations on the constructed object may fail
            /// if the user provided lastWriteTime doesn't match the file on the disk.</param>
            /// <returns>Newly created font file reference object, or NULL in case of failure.</returns>
            IDWriteFontFile CreateFontFileReference(string filePath, IntPtr lastWriteTime);

            /// <summary>
            /// CreateCustomFontFileReference creates a reference to an application specific font file resource.
            /// This function enables an application or a document to use a font without having to install it on the system.
            /// The fontFileReferenceKey has to be unique only in the scope of the fontFileLoader used in this call.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateCustomFontFileReference();

            /// <summary>
            /// Creates a font face object.
            /// </summary>
            /// <param name="fontFaceType">The file format of the font face.</param>
            /// <param name="numberOfFiles">The number of font files required to represent the font face.</param>
            /// <param name="fontFiles">Font files representing the font face. Since IDWriteFontFace maintains its own references
            /// to the input font file objects, it's OK to release them after this call.</param>
            /// <param name="faceIndex">The zero based index of a font face in cases when the font files contain a collection of font faces.
            /// If the font files contain a single face, this value should be zero.</param>
            /// <param name="fontFaceSimulationFlags">Font face simulation flags for algorithmic emboldening and italicization.</param>
            /// <returns>Contains the newly created font face object, or NULL in case of failure.</returns>
            IDWriteFontFace CreateFontFace(FontFaceType fontFaceType, int numberOfFiles, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IDWriteFontFile[] fontFiles, int faceIndex, FontSimulations fontFaceSimulationFlags);

            /// <summary>
            /// Creates a rendering parameters object with default settings for the primary monitor.
            /// </summary>
            /// <returns>The newly created rendering parameters object, or NULL in case of failure.</returns>
            IDWriteRenderingParams CreateRenderingParams();


            /// <summary>
            /// Creates a rendering parameters object with default settings for the specified monitor.
            /// </summary>
            /// <param name="monitor">The monitor to read the default values from.</param>
            /// <returns>The newly created rendering parameters object, or NULL in case of failure.</returns>
            IDWriteRenderingParams CreateMonitorRenderingParams(IntPtr monitor);


            /// <summary>
            /// Creates a rendering parameters object with the specified properties.
            /// </summary>
            /// <param name="gamma">The gamma value used for gamma correction, which must be greater than zero and cannot exceed 256.</param>
            /// <param name="enhancedContrast">The amount of contrast enhancement, zero or greater.</param>
            /// <param name="clearTypeLevel">The degree of ClearType level, from 0.0f (no ClearType) to 1.0f (full ClearType).</param>
            /// <param name="pixelGeometry">The geometry of a device pixel.</param>
            /// <param name="renderingMode">Method of rendering glyphs. In most cases, this should be DWRITE_RENDERING_MODE_DEFAULT to automatically use an appropriate mode.</param>
            /// <returns>The newly created rendering parameters object, or NULL in case of failure.</returns>
            IDWriteRenderingParams CreateCustomRenderingParams(float gamma, float enhancedContrast, float clearTypeLevel, PixelGeometry pixelGeometry, RenderingMode renderingMode);

            /// <summary>
            /// Registers a font file loader with DirectWrite.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_RegisterFontFileLoader();

            /// <summary>
            /// Unregisters a font file loader that was previously registered with the DirectWrite font system using RegisterFontFileLoader.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_UnregisterFontFileLoader();

            /// <summary>
            /// Create a text format object used for text layout.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateTextFormat();

            /// <summary>
            /// Create a typography object used in conjunction with text format for text layout.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateTypography();

            /// <summary>
            /// Create an object used for interoperability with GDI.
            /// </summary>
            /// <returns>The GDI interop object if successful, or NULL in case of failure.</returns>
            IDWriteGdiInterop GetGdiInterop();

            /// <summary>
            /// CreateTextLayout takes a string, format, and associated constraints
            /// and produces an object representing the fully analyzed
            /// and formatted result.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateTextLayout();

            /// <summary>
            /// CreateGdiCompatibleTextLayout takes a string, format, and associated constraints
            /// and produces and object representing the result formatted for a particular display resolution
            /// and measuring mode. The resulting text layout should only be used for the intended resolution,
            /// and for cases where text scalability is desired, CreateTextLayout should be used instead.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateGdiCompatibleTextLayout();

            /// <summary>
            /// The application may call this function to create an inline object for trimming, using an ellipsis as the omission sign.
            /// The ellipsis will be created using the current settings of the format, including base font, style, and any effects.
            /// Alternate omission signs can be created by the application by implementing IDWriteInlineObject.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateEllipsisTrimmingSign();

            /// <summary>
            /// Return an interface to perform text analysis with.
            /// </summary>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateTextAnalyzer();

            /// <summary>
            /// Creates a number substitution object using a locale name,
            /// substitution method, and whether to ignore user overrides (uses NLS
            /// defaults for the given culture instead).
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateNumberSubstitution();

            /// <summary>
            /// Creates a glyph run analysis object, which encapsulates information
            /// used to render a glyph run.
            /// </summary>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_CreateGlyphRunAnalysis();
        }
    }
}
