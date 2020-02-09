namespace Terka.FontBuilder.Parser.Reflection.Extensions
{
    /// <summary>
    /// The extensions to access non-public members of objects.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a wrapper which has accessible non-public properties.
        /// </summary>
        /// <param name="o">
        /// The oject to wrap.
        /// </param>
        /// <returns>
        /// The dynamic.
        /// </returns>
        public static dynamic AccessNonPublic(this object o)
        {
            return new AccessPrivateWrapper(o);
        }
    }
}