using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    public partial class StateMachineAppendix
    {
        /// <summary>
        /// Position change heap parameters.
        /// </summary>
        public class PositionChangesParameters : HeapParameters
        {
            private byte _count;
            private List<PositionChange> _positionChanges;

            /// <summary>
            /// Gets or sets how many position changes are.
            /// </summary>
            public byte Count
            {
                get { return _count; }
                set { _count = value; }
            }
            /// <summary>
            /// Gets list of position changes.
            /// </summary>
            public List<PositionChange> PositionChanges
            {
                get { return _positionChanges; }
            }

            /// <summary>
            /// Creates new instance of position change parameters.
            /// </summary>
            public PositionChangesParameters()
            {
                _positionChanges = new List<PositionChange>();
            }

            /// <summary>
            /// Reads parameters from byte array <paramref name="data"/> starting from zero-base <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Byte array containing heap.</param>
            /// <param name="offset">Zero-based offset to <paramref name="data"/> heap.</param>
            public override void ReadFrom(byte[] data, int offset)
            {
                _positionChanges.Clear();

                _count = data[offset++];

                for (int i = 0; i < _count; i++, offset += PositionChange.SizeOf)
                {
                    PositionChange position = new PositionChange();
                    position.ReadFrom(data, offset);

                    _positionChanges.Add(position);
                }
            }

            /// <summary>
            /// Writes parameters to byte array <paramref name="data"/> starting on zero-based <paramref name="offset"/>.
            /// </summary>
            /// <param name="data">Heap data.</param>
            /// <param name="offset">Starting offset in <paramref name="data"/>.</param>
            public override void WriteTo(byte[] data, int offset)
            {
                data[offset++] = _count;

                for (int i = 0; i < _count; i++)
                {
                    _positionChanges[i].WriteTo(data, offset);
                    offset += PositionChange.SizeOf;
                }
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public override void Update()
            {
                UpdateAssert(_positionChanges.Count <= byte.MaxValue, "Too many position changes.");

                _count = (byte)_positionChanges.Count;
            }
            private static void UpdateAssert(bool condition, string error)
            {
                if (condition == false)
                    throw new InvalidOperationException(error);
            }

            /// <summary>
            /// Gets size in bytes of parameters.
            /// </summary>
            /// <returns>Size in bytes.</returns>
            public override int GetSize()
            {
                return sizeof(byte) + _positionChanges.Count * PositionChange.SizeOf;
            }
        }
    }
}
