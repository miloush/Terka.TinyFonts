namespace Terka.TinyFonts
{
    using System;
    using System.ComponentModel;
    using System.IO;

    partial class StateMachineAppendix
    {
        /// <summary>
        /// Conditions are represented by unsigned numbers.
        /// </summary>
        public enum RuleCondition : byte
        {
            /// <summary>
            /// Rule is always applied.
            /// </summary>
            Unconditional = 0,
            /// <summary>
            /// Rule is applied if and only if the glyph is of specified number.
            /// </summary>
            Glyph = 1,
            /// <summary>
            /// Rule is applied if and only if the glyph belongs to specific group class.
            /// </summary>
            GlyphClass = 2,
            /// <summary>
            /// Rule is applied if and only if the user required specific feature. For details, see Rules organization.
            /// </summary>
            Feature = 3,
            /// <summary>
            /// Rule is applied if and only if the position of condition is not valid.
            /// </summary>
            OutOfTape = 4,
            /// <summary>
            /// Rule is applied if and only if the glyph has a specified property.
            /// </summary>
            GlyphProperty = 5,
        }

        /// <summary>
        /// Actions for substitution machines.
        /// </summary>
        public enum RuleAction : byte
        {
            /// <summary>
            /// No action is performed. Such rules can be used when only changing the state and/or advancing the tape is desired.
            /// </summary>
            Nothing = 0,
            /// <summary>
            /// The glyph number will be replaced by the specified glyph number.
            /// </summary>
            GlyphOverwrite = 1,
            /// <summary>
            /// Two consecutive glyph numbers will be replaced by the specified glyph number.
            /// </summary>
            GlyphRewrite_2_1 = 2,
            /// <summary>
            /// Three consecutive glyph numbers will be replaced by the specified glyph number.
            /// </summary>
            GlyphRewrite_3_1 = 3,
            /// <summary>
            /// N consecutive glyphs will be replaced by M ones, as defined in the parameters heap.
            /// </summary>
            GlyphRewrite_N_M = 4,
            /// <summary>
            /// The specified number of glyphs will be deleted.
            /// </summary>
            GlyphDeletion = 5,
            /// <summary>
            /// The specified glyph will be inserted.
            /// </summary>
            GlyphInsertion = 6,
            /// <summary>
            /// Glyph reorder Ax→xA.      
            /// </summary>
            GlyphReorder_Ax_xA = 7,
            /// <summary>
            /// Glyph reorder xD→Dx.      
            /// </summary>
            GlyphReorder_xD_Dx = 8,
            /// <summary>
            /// Glyph reorder AxD→DxA.    
            /// </summary>
            GlyphReorder_AxD_DxA = 9,
            /// <summary>
            /// Glyph reorder ABx→xAB.    
            /// </summary>
            GlyphReorder_ABx_xAB = 10,
            /// <summary>
            /// Glyph reorder xCD→CDx.    
            /// </summary>
            GlyphReorder_xCD_CDx = 11,
            /// <summary>
            /// Glyph reorder xCD→DCx.    
            /// </summary>
            GlyphReorder_xCD_DCx = 12,
            /// <summary>
            /// Glyph reorder AxCD→CDxA.  
            /// </summary>
            GlyphReorder_AxCD_CDxA = 13,
            /// <summary>
            /// Glyph reorder AxCD→DCxA.  
            /// </summary>
            GlyphReorder_AxCD_DCxA = 14,
            /// <summary>
            /// Glyph reorder ABxD→DxAB.  
            /// </summary>
            GlyphReorder_ABxD_DxAB = 15,
            /// <summary>
            /// Glyph reorder ABxD→DxBA.  
            /// </summary>
            GlyphReorder_ABxD_DxBA = 16,
            /// <summary>
            /// Glyph reorder ABxCD→CDxAB.
            /// </summary>
            GlyphReorder_ABxCD_CDxAB = 17,
            /// <summary>
            /// Glyph reorder ABxCD→CDxBA.
            /// </summary>
            GlyphReorder_ABxCD_CDxBA = 18,
            /// <summary>
            /// Glyph reorder ABxCD→DCxAB.
            /// </summary>
            GlyphReorder_ABxCD_DCxAB = 19,
            /// <summary>
            /// Glyph reorder ABxCD→DCxBA.</summary>
            GlyphReorder_ABxCD_DCxBA = 20,
            /// <summary>
            /// Relative change to glyph offset.
            /// </summary>
            PositionOffset = 128,
            /// <summary>
            /// Relative change to glyph advances.
            /// </summary>
            PositionAdvance = 129,
            /// <summary>
            /// Complex relative change of glyph's positions, mutliple changes or offsets and advances together.
            /// </summary>
            PositionComplex = 130,
        }

        /// <summary>
        /// Class representing one state machine r
        /// </summary>
        public class Rule
        {
            internal const int SizeOf = sizeof(ushort) + sizeof(ushort) +
                                        sizeof(RuleCondition) + sizeof(sbyte) + sizeof(ushort) +
                                        sizeof(RuleAction) + sizeof(sbyte) + sizeof(ushort) +
                                        sizeof(sbyte) + 3;

            private ushort _requiredState;
            private ushort _newState;

            private RuleCondition _condition;
            private sbyte _conditionOffset;
            private ushort _conditionParameter;

            private RuleAction _action;
            private sbyte _actionOffset;
            private ushort _actionParameter;

            private sbyte _tapeMovement;
            private byte _conditionPlane;
            private byte _actionPlane;
            private byte _reserved3;

            /// <summary>
            /// Gets or sets required state.
            /// </summary>
            public ushort RequiredState
            {
                get { return _requiredState; }
                set { _requiredState = value; }
            }
            /// <summary>
            /// Gets or sets new state.
            /// </summary>
            public ushort NewState
            {
                get { return _newState; }
                set { _newState = value; }
            }

            /// <summary>
            /// Gets or sets condition for this rule.
            /// </summary>
            public RuleCondition Condition
            {
                get { return _condition; }
                set { _condition = value; }
            }
            /// <summary>
            /// Gets or sets condititon offset.
            /// </summary>
            public sbyte ConditionOffset
            {
                get { return _conditionOffset; }
                set { _conditionOffset = value; }
            }
            /// <summary>
            /// Gets or sets condition parameter.
            /// </summary>
            public ushort ConditionParameter
            {
                get { return _conditionParameter; }
                set { _conditionParameter = value; }
            }

            /// <summary>
            /// Gets or sets rule action.
            /// </summary>
            public RuleAction Action
            {
                get { return _action; }
                set { _action = value; }
            }
            /// <summary>
            /// Gets or sets action offset.
            /// </summary>
            public sbyte ActionOffset
            {
                get { return _actionOffset; }
                set { _actionOffset = value; }
            }
            /// <summary>
            /// Gets or sets action parameter.
            /// </summary>
            public ushort ActionParameter
            {
                get { return _actionParameter; }
                set { _actionParameter = value; }
            }

            /// <summary>
            /// Gets or sets tape movement.
            /// </summary>
            public sbyte TapeMovement
            {
                get { return _tapeMovement; }
                set { _tapeMovement = value; }
            }

            /// <summary>
            /// Gets or sets plane of condition part.
            /// </summary>
            public byte ConditionPlane
            {
                get { return _conditionPlane; }
                set { _conditionPlane = value; }
            }

            /// <summary>
            /// Gets or sets plane of action part.
            /// </summary>
            public byte ActionPlane
            {
                get { return _actionPlane; }
                set { _actionPlane = value; }
            }
            
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public byte Reserved3
            {
                get { return _reserved3; }
                set { _reserved3 = value; }
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            public void ReadFrom(BinaryReader reader)
            {
                _requiredState = reader.ReadUInt16();
                _newState = reader.ReadUInt16();

                _condition = (RuleCondition)reader.ReadByte();
                _conditionOffset = reader.ReadSByte();
                _conditionParameter = reader.ReadUInt16();

                _action = (RuleAction)reader.ReadByte();
                _actionOffset = reader.ReadSByte();
                _actionParameter = reader.ReadUInt16();

                _tapeMovement = reader.ReadSByte();
                _conditionPlane = reader.ReadByte();
                _actionPlane = reader.ReadByte();
                _reserved3 = reader.ReadByte();
            }

            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(_requiredState);
                writer.Write(_newState);

                writer.Write((byte)_condition);
                writer.Write(_conditionOffset);
                writer.Write(_conditionParameter);

                writer.Write((byte)_action);
                writer.Write(_actionOffset);
                writer.Write(_actionParameter);

                writer.Write(_tapeMovement);
                writer.Write(_conditionPlane);
                writer.Write(_actionPlane);
                writer.Write(_reserved3);
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return string.Format("{0}, ({1}, {3}, {2}) → {4}, ({5}, {7}, {6}); {8}", _requiredState, _condition, _conditionOffset, _conditionParameter, _newState, _action, _actionOffset, _actionParameter, _tapeMovement);
            }
        }
    }
}
