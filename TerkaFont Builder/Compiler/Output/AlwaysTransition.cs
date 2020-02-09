namespace Terka.FontBuilder.Compiler.Output
{
    using System.Collections.Generic;

    /// <summary>
    /// Transition which can be always traversed.
    /// </summary>
    public class AlwaysTransition : ITransition
    {
        /// <inheritdoc />
        public State TargetState { get; set; }

        /// <inheritdoc />
        public bool IsFallback 
        { 
            get 
            {
                return true;
            }
        }

        /// <inheritdoc />
        public int SortingKey 
        { 
            get
            {
                // This transition whould be always at the end of the sequence (along with fallback transitions).
                return int.MaxValue;
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
            return true;
        }

        /// <inheritdoc />
        public ITransition Clone()
        {
            return new AlwaysTransition
            {
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
                271,
                this.GetType(),
                this.HeadShift,
                this.LookupFlags,
                this.Action != null ? this.Action.GetHashCode() : 0
            );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "(*)";
        }
    }
}
