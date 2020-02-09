using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    public partial class StateMachineAppendix
    {
        /// <summary>
        /// Describes glyph position changes.
        /// </summary>
        public class PositionChange
        {
            internal const int SizeOf = sizeof(sbyte) + sizeof(sbyte) + sizeof(sbyte) + sizeof(sbyte);

            private sbyte _offsetX;
            private sbyte _offsetY;
            private sbyte _advanceX;
            private sbyte _advanceY;

            /// <summary>
            /// Gets or sets X-offset relative position change.
            /// </summary>
            public sbyte OffsetX
            {
                get { return _offsetX; }
                set { _offsetX = value; }
            }
            /// <summary>
            /// Gets or sets Y-offset relative position change.
            /// </summary>
            public sbyte OffsetY
            {
                get { return _offsetY; }
                set { _offsetY = value; }
            }
            /// <summary>
            /// Gets or sets X-advance relative position change.
            /// </summary>
            public sbyte AdvanceX
            {
                get { return _advanceX; }
                set { _advanceX = value; }
            }
            /// <summary>
            /// Gets or sets Y-advance relative position change.
            /// </summary>
            public sbyte AdvanceY
            {
                get { return _advanceY; }
                set { _advanceY = value; }
            }

            /// <summary>
            /// Creates new instance.
            /// </summary>
            public PositionChange()
            {
            }

            /// <summary>
            /// Creates new instance with predefined values.
            /// </summary>
            /// <param name="offsetX">Relative X-offset change.</param>
            /// <param name="offsetY">Relative Y-offset change.</param>
            /// <param name="advanceX">Relative X-advance change.</param>
            /// <param name="advanceY">Relative Y-advance change.</param>
            public PositionChange(sbyte offsetX, sbyte offsetY, sbyte advanceX, sbyte advanceY)
            {
                _offsetX = offsetX;
                _offsetY = offsetY;
                _advanceX = advanceX;
                _advanceY = advanceY;
            }

            /// <summary>
            /// Reads parameters from byte array <paramref name="heap"/> starting from zero-base <paramref name="offset"/>.
            /// </summary>
            /// <param name="heap">Byte array containing heap.</param>
            /// <param name="offset">Zero-based offset to <paramref name="heap"/> heap.</param>
            public void ReadFrom(byte[] heap, int offset)
            {
                _offsetX = (sbyte)heap[offset++];
                _offsetY = (sbyte)heap[offset++];
                _advanceX = (sbyte)heap[offset++];
                _advanceY = (sbyte)heap[offset++];
            }

            /// <summary>
            /// Writes parameters to byte array <paramref name="heap"/> starting on zero-based <paramref name="offset"/>.
            /// </summary>
            /// <param name="heap">Heap data.</param>
            /// <param name="offset">Starting offset in <paramref name="heap"/>.</param>
            public void WriteTo(byte[] heap, int offset)
            {
                heap[offset++] = (byte)_offsetX;
                heap[offset++] = (byte)_offsetY;
                heap[offset++] = (byte)_advanceX;
                heap[offset++] = (byte)_advanceY;
            }
        }
    }
}
