using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terka.TinyFonts
{
    /// <summary>
    /// Font appendix for positioning state machine.
    /// </summary>
    public class PositioningAppendix : StateMachineAppendix
    {
        /// <summary>
        /// Appendix for positioninig information (from GPOS OpenType table).
        /// </summary>
        public PositioningAppendix() : base(PositioningMachine)
        {

        }
    }
}
