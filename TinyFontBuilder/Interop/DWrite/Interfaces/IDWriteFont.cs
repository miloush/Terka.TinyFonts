namespace Win32
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The IDWriteFont interface represents a physical font in a font collection.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteFont), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDWriteFont
        {
            /// <summary>
            /// Gets the font family to which the specified font belongs.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetFontFamily();

            /// <summary>
            /// Gets the weight of the specified font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetWeight();

            /// <summary>
            /// Gets the stretch (aka. width) of the specified font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetStretch();

            /// <summary>
            /// Gets the style (aka. slope) of the specified font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetStyle();

            /// <summary>
            /// Returns TRUE if the font is a symbol font or FALSE if not.
            /// </summary>
            [PreserveSig]
            bool IsSymbolFont();

            /// <summary>
            /// Gets a localized strings collection containing the face names for the font (e.g., Regular or Bold), indexed by locale name.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetFaceNames();

            /// <summary>
            /// Gets a localized strings collection containing the specified informational strings, indexed by locale name.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetInformationalStrings();

            /// <summary>
            /// Gets a value that indicates what simulation are applied to the specified font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetSimulations();

            /// <summary>
            /// Gets the metrics for the font.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            object PlaceHolder_GetMetrics();

            /// <summary>
            /// Determines whether the font supports the specified character.
            /// </summary>
            /// <param name="unicodeValue">Unicode (UCS-4) character value.</param>
            /// <returns>TRUE if the font supports the specified character or FALSE if not.</returns>
            bool HasCharacter(uint unicodeValue);

            /// <summary>
            /// Creates a font face object for the font.
            /// </summary>
            /// <returns>The newly created font face object.</returns>
            IDWriteFontFace CreateFontFace();
        }
    }
}
