namespace Terka.FontBuilder.Compiler.Output
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="StateMachine"/> transition, which is valid for multiple glyph IDs.
    /// </summary>
    public class SetTransition : ITransition
    {
        /// <summary>
        /// Gets the set of glyph IDs for which this transition is valid.
        /// </summary>
        /// <value>
        /// The glyph ID set.
        /// </value>
        public HashSet<ushort> GlyphIdSet { get; set; }

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
                return this.GlyphIdSet.Count > 0 ? this.GlyphIdSet.Min() : -1;
            }
        }

        /// <inheritdoc />
        public int HeadShift { get; set; }

        /// <inheritdoc />
        public LookupFlags LookupFlags { get; set; }

        /// <inheritdoc />
        public ITransitionAction Action { get; set; }

        /// <inheritdoc />
        public bool IsGlyphIdMatching(ushort glyphId)
        {
            return this.GlyphIdSet.Contains(glyphId);
        }

        /// <inheritdoc />
        public ITransition Clone()
        {
            return new SetTransition
            {
                GlyphIdSet = new HashSet<ushort>(this.GlyphIdSet),
                Action = this.Action == null ? null : this.Action.Clone(),
                HeadShift = this.HeadShift,
                LookupFlags = this.LookupFlags
            };
        }

        /// <inheritdoc />
        public IEnumerable<ITransition> GetUngroupedTransitions()
        {
            return this.GlyphIdSet.Select(glyphId => new SimpleTransition {
                GlyphId = glyphId,
                HeadShift = this.HeadShift,
                Action = this.Action != null ? this.Action.Clone() : null,
                LookupFlags = this.LookupFlags
            });
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeBuilder.BuildHashCode(
                251,
                this.GetType(),
                this.GlyphIdSet,
                this.HeadShift,
                this.LookupFlags,
                this.Action != null ? this.Action.GetHashCode() : 0
            );
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "(" + string.Join(", ", this.GlyphIdSet.Select(p => p.ToString(CultureInfo.InvariantCulture))) + ")";
        }
    }
}