namespace Win32
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The interface that represents a reference to a font file.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteFontFile), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDWriteFontFile
        {   
            /// <summary>
            /// This method obtains the pointer to the reference key of a font file. The pointer is only valid until the object that refers to it is released.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetReferenceKey();

            /// <summary>
            /// Obtains the file loader associated with a font file object.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetLoader();

            /// <summary>
            /// Analyzes a file and returns whether it represents a font, and whether the font type is supported by the font system.
            /// </summary>
            /// <param name="isSupportedFontType">TRUE if the font type is supported by the font system, FALSE otherwise.</param>
            /// <param name="fontFileType">The type of the font file. Note that even if isSupportedFontType is FALSE,
            /// the fontFileType value may be different from DWRITE_FONT_FILE_TYPE_UNKNOWN.</param>
            /// <param name="fontFaceType">The type of the font face that can be constructed from the font file.
            /// Note that even if isSupportedFontType is FALSE, the fontFaceType value may be different from
            /// DWRITE_FONT_FACE_TYPE_UNKNOWN.</param>
            /// <param name="numberOfFaces">Number of font faces contained in the font file.</param>
            /// <remarks>
            /// IMPORTANT: certain font file types are recognized, but not supported by the font system.
            /// For example, the font system will recognize a file as a Type 1 font file,
            /// but will not be able to construct a font face object from it. In such situations, Analyze will set
            /// isSupportedFontType output parameter to FALSE.
            /// </remarks>
            void Analyze(out bool isSupportedFontType, out FontFileType fontFileType, out FontFaceType fontFaceType, out uint numberOfFaces);
        }
    }
}
