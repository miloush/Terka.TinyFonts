namespace Terka.TinyFonts
{
    using System;

    partial class StateMachineAppendix
    {
        /// <summary>
        /// Flags for executing features.
        /// </summary>
        [Flags]
        public enum FeatureFlags : byte
        {
            /// <summary>
            /// None.
            /// </summary>
            None,
            /// <summary>
            /// Feature is executed in opposite direction.
            /// </summary>
            Reverse = 1,
        }
    }
}