namespace Terka.FontBuilder
{
    using System.Collections;
    
    //// ReSharper disable LoopCanBeConvertedToQuery

    /// <summary>
    /// Utilties to help build implementations of <see cref="object.GetHashCode"/>.
    /// </summary>
    public static class HashCodeBuilder
    {
        /// <summary>
        /// Generates a hash code from a series of objects based on a specific prime number.
        /// </summary>
        /// <param name="prime">The prime.</param>
        /// <param name="objs">The objs.</param>
        /// <returns>The hash code.</returns>
        public static int BuildHashCode(int prime, params object[] objs)
        {
            return BuildHashCodeForCollection(prime, objs);
        }

        private static int BuildHashCodeForCollection(int prime, IEnumerable objs)
        {
            int result = 0;

            foreach (object o in objs)
            {
                result = unchecked((result + ((o.GetType().IsArray || o is IEnumerable) ? 
                    BuildHashCodeForCollection(prime, (IEnumerable)o) : 
                    o.GetHashCode())) * prime);
            }
            return result;
        }
    }
}
