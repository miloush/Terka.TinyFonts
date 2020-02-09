using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    partial class StateMachineAppendix
    {
        /// <summary>
        /// Base class for heap parameters which are stored in global appendix heap.
        /// </summary>
        public abstract class HeapParameters
        {
            /// <summary>
            /// Reads parameters from byte array <paramref name="data"/> starting from zero-base <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Byte array containing heap.</param>
            /// <param name="offset">Zero-based offset to <paramref name="data"/> heap.</param>
            public abstract void ReadFrom(byte[] data, int offset);
            /// <summary>
            /// Writes parameters to byte array <paramref name="data"/> starting on zero-based <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Heap data.</param>
            /// <param name="offset">Starting offset in <paramref name="data"/>.</param>
            public abstract void WriteTo(byte[] data, int offset);
            /// <summary>
            /// Upd
            /// </summary>
            public abstract void Update();
            /// <summary>
            /// Gets size in bytes of parameters.
            /// </summary>
            /// <returns>Size in bytes.</returns>
            public abstract int GetSize();
        }
    }
}
