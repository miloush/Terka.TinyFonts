namespace Terka.TinyFonts
{
    /// <summary>
    /// Antialiasing level for characters.
    /// </summary>
    public enum AntialiasingLevel
    {
        /// <summary>
        /// No antialiasing.
        /// </summary>
        None = 1,

        /// <summary>
        /// Antialiasing with 5 shades of gray.
        /// </summary>
        Gray5 = 2,

        /// <summary>
        /// Antialiasing with 17 shades of gray.
        /// </summary>
        Gray17 = 4,

        /// <summary>
        /// Antialiasing with 65 shades of gray.
        /// </summary>
        Gray65 = 8
    }
}
