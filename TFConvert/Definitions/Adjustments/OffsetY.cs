namespace Terka.TinyFonts.TFConvert
{
    /// <summary>
    /// Shifts a character or range of characters up or down.
    /// </summary>
    /// <remarks>
    /// The OffsetY command applies to the characters specified by any
    /// <see cref="ImportRange" />, <see cref="ImportRangeAndMap" />, or <see cref="SetAsDefaultCharacter" />
    /// commands that follow it in the definition and precede any subsequent OffsetY commands in the same definition.
    /// </remarks>
    [TinyCommand]
    public class OffsetY : TinyAdjustmentCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetY"/> command.
        /// </summary>
        public OffsetY()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetY"/> command.
        /// </summary>
        /// <param name="adjustment">
        /// An integer indicating the number of EM units to shift a character up or down.
        /// A positive number shifts the characters down, and a negative number shifts characters up.
        /// </param>
        public OffsetY(short adjustment) : base(adjustment)
        {
        }
    }
}
