using System;

namespace Terka.FontBuilder
{
    /// <summary>
    /// Corresponds to OT "LookupFlag".
    /// </summary>
    [Flags]
    public enum LookupFlags : ushort
    {
        /// <summary>
        /// No flag is enabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Corresponds to OT "RightToLeft" lookup flag.
        /// </summary>
        RightToLeft = 1,

        /// <summary>
        /// Corresponds to OT "IgnoreBaseGlyphs" lookup flag.
        /// </summary>
        IgnoreBaseGlyphs = 2,

        /// <summary>
        /// Corresponds to OT "IgnoreLigatures" lookup flag.
        /// </summary>
        IgnoreLigatures = 4,

        /// <summary>
        /// Corresponds to OT "IgnoreMarks" lookup flag.
        /// </summary>
        IgnoreMarks = 8,

        /// <summary>
        /// Corresponds to OT "UseMarkFilteringSet" lookup flag.
        /// </summary>
        UseMarkFilteringSet = 16,

        /// <summary>
        /// Corresponds to OT "MarkAttachmentType" lookup flag.
        /// </summary>
        MarkAttachmentTypeMask = 0xFF00
    }
}
