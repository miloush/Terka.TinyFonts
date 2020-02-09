
namespace Terka.TinyFonts
{
    /// <summary>
    /// Font appendix for substitution state machine.
    /// </summary>
    public class SubstitutionAppendix : StateMachineAppendix
    {
        /// <summary>
        /// Appendix for substitutioning information (from GSUB OpenType table).
        /// </summary>
        public SubstitutionAppendix()
            : base(SubstitutionMachine)
        {
        }
    }
}
