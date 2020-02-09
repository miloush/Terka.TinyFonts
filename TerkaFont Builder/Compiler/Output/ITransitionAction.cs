namespace Terka.FontBuilder.Compiler.Output
{
    public interface ITransitionAction
    {
        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        ITransitionAction Clone();
    }
}