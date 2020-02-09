namespace Terka.TinyResources
{
    /// <summary>
    /// Type of Tiny Resource.
    /// </summary>
    public enum TinyResourceKind : byte
    {
        /// <summary>
        /// Invalid type of resource.
        /// </summary>
        Invalid,
        /// <summary>
        /// Bitmap resource.
        /// </summary>
        Bitmap,
        /// <summary>
        /// Font resource.
        /// </summary>
        Font,
        /// <summary>
        /// String resource.
        /// </summary>
        String,
        /// <summary>
        /// Binary resource.
        /// </summary>
        Binary
    }
}
