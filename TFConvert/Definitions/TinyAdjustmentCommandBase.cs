namespace Terka.TinyFonts.TFConvert
{
    // [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields do not corrupt state and distingush computed values.")]

    /// <summary>
    /// Base class for commands having the Adjustment integer parameter.
    /// </summary>
    public abstract class TinyAdjustmentCommandBase : TinyCommandBase
    {
        /// <summary>
        /// An integer indicating the number of units to adjust the standard value.
        /// </summary>
        [TinyParameter]
        public short Adjustment;

        /// <summary>
        /// Initializes a new instance of an adjustment command with specified adjusment.
        /// </summary>
        /// <param name="adjustment">The adjustment value.</param>
        public TinyAdjustmentCommandBase(short adjustment)
        {
            this.Adjustment = adjustment;
        }

        /// <summary>
        /// Initializes a new instance of an adjustment command with no adjustment.
        /// </summary>
        protected TinyAdjustmentCommandBase()
        {
        }
    }
}
