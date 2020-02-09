namespace Terka.TinyBitmaps
{
    using System;

    /// <summary>
    /// The individual flags for bitmap description.
    /// </summary>
    [Flags]
    public enum BitmapDescriptionFlags : ushort
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None,
        /// <summary>
        /// Read only.
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// Compressed.
        /// </summary>
        Compressed = 2
    }
}
