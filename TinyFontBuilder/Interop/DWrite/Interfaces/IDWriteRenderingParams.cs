namespace Win32
{
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        /// <summary>
        /// The interface that represents text rendering settings for glyph rasterization and filtering.
        /// </summary>
        [ComImport, Guid(UuidOf.IDWriteRenderingParams), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDWriteRenderingParams
        {
            /// <summary>
            /// Gets the gamma value used for gamma correction. Valid values must be
            /// greater than zero and cannot exceed 256.
            /// </summary>
            [PreserveSig]
            float GetGamma();

            /// <summary>
            /// Gets the amount of contrast enhancement. Valid values are greater than
            /// or equal to zero.
            /// </summary>
            [PreserveSig]
            float GetEnhancedContrast();

            /// <summary>
            /// Gets the ClearType level. Valid values range from 0.0f (no ClearType) 
            /// to 1.0f (full ClearType).
            /// </summary>
            [PreserveSig]
            float GetClearTypeLevel();

            /// <summary>
            /// Gets the rendering mode.
            /// </summary>
            [PreserveSig]
            RenderingMode GetRenderingMode();
        }
    }
}
