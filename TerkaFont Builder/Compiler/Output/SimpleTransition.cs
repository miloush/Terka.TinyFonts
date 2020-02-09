namespace Terka.FontBuilder.Compiler.Output
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a <see cref="StateMachine"/> transition, which is valid for a single glyph ID.
    /// </summary>
    public class SimpleTransition : ITransition
    {
        /// <summary>
        /// Gets or the glyph ID for which this transition is valid.
        /// </summary>
        /// <value>
        /// The glyph ID.
        /// </value>
        public ushort GlyphId { get; set; }

        /// <inheritdoc />
        public State TargetState { get; set; }

        /// <inheritdoc />
        public bool IsFallback
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public int SortingKey
        {
            get
            {
                return this.GlyphId;
            }
        }

        public int HeadShift { get; set; }

        public LookupFlags LookupFlags { get; set; }

        public ITransitionAction Action { get; set; }

        /// <inheritdoc />
        public bool IsGlyphIdMatching(ushort glyphId)
        {
            return glyphId == this.GlyphId;
        }

        /// <inheritdoc />
        public ITransition Clone()
        {
            return new SimpleTransition
            {
                GlyphId = this.GlyphId,
                HeadShift = this.HeadShift,
                LookupFlags = this.LookupFlags,
                Action = this.Action == null ? null : this.Action.Clone()
            };
        }

        /// <inheritdoc />
        public IEnumerable<ITransition> GetUngroupedTransitions()
        {
            return new[] { this.Clone() };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(
                67,
                this.GetType(),
                this.GlyphId,
                this.HeadShift,
                this.LookupFlags,
                this.Action != null ? this.Action.GetHashCode() : 0
            );
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "(" + this.GlyphId + ")";
        }
    }
}