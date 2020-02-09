namespace Terka.TinyFonts
{
    using System;
    using System.ComponentModel;
    using System.IO;

    partial class StateMachineAppendix
    {
        /// <summary>
        /// Feature offset description.
        /// </summary>
        public class FeatureOffset : IComparable<FeatureOffset>
        {
            internal const int SizeOf = sizeof(uint) + sizeof(uint);

            private uint _tag;
            private uint _offset;
            private FeatureFlags _flags;

            /// <summary>
            /// Gets or sets corresponding feature tag from OpenType.
            /// </summary>
            public uint Tag
            {
                get { return _tag; }
                set { _tag = value; }
            }

            /// <summary>
            /// Gets or sets the offset to the feature data.
            /// </summary>
            public uint Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }

            /// <summary>
            /// Gets or sets flags for feature.
            /// </summary>
            public FeatureFlags Flags
            {
                get { return _flags; }
                set { _flags = value; }
            }

            /// <summary>
            /// Gets or sets reverse flag for feature.
            /// </summary>
            public bool IsReverse
            {
                get { return _flags.HasFlag(FeatureFlags.Reverse); }
                set { _flags = (FeatureFlags)Helper.SetFlag((int)_flags, (int)FeatureFlags.Reverse, value); }
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            public void ReadFrom(BinaryReader reader)
            {
                _tag = reader.ReadUInt32();

                uint offsetWithFlags = reader.ReadUInt32();
                _flags = (FeatureFlags)offsetWithFlags;
                _offset = offsetWithFlags >> 8;

            }
            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(_tag);

                uint offsetWithFlags = (uint)((byte)_flags + (_offset << 8));
                writer.Write(offsetWithFlags);
            }

            int IComparable<FeatureOffset>.CompareTo(FeatureOffset other)
            {
                return Tag.CompareTo(other.Tag);
            }
        }
    }
}
