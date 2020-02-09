using System.Collections.Generic;
using System.Linq;

namespace Terka.FontBuilder.Compiler.Output
{
    using System.Runtime.CompilerServices;

    public class State
    {
        public State()
        {
            this.Transitions = new List<ITransition>();
        }

        /// <inheritdoc />
        public IList<ITransition> Transitions { get; set; }

        /// <inheritdoc />
        public ITransition GetTransitionByGlyphId(ushort glyphId, bool fallback = false)
        {
            return this.Transitions.FirstOrDefault(transition => transition.IsGlyphIdMatching(glyphId) && transition.IsFallback == fallback);
        }

        public int DebugInstanceId
        {
            get
            {
                return RuntimeHelpers.GetHashCode(this);
            }            
        }

        public override string ToString()
        {
            return "State #" + this.DebugInstanceId;
        }
    }
}
