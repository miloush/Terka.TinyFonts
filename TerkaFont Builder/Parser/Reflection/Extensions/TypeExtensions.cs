namespace Terka.FontBuilder.Parser.Reflection.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Instantiates a type.
        /// </summary>
        /// <param name="t">
        /// The type.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The instance.
        /// </returns>
        public static object Instantiate(this Type t, params object[] args)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, args.Select(a => a.GetType()).ToArray(), null);

            if (ctor == null)
            {
                throw new InvalidOperationException(t.Name + " has no public ctor accepting types " + string.Join(", ", args.Select(p => p.GetType().Name)));
            }

            return ctor.Invoke(args);
        }

        /// <summary>
        /// Instantiates a type using non-public constructor.
        /// </summary>
        /// <param name="t">
        /// The type.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The instance.
        /// </returns>
        public static object InstantiateNonPublic(this Type t, params object[] args)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var ctor = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, args.Select(a => a.GetType()).ToArray(), null);

            if (ctor == null)
            {
                throw new InvalidOperationException(t.Name + " has no non-public ctor accepting types " + string.Join(", ", args.Select(p => p.GetType().Name)));
            }

            return ctor.Invoke(args);
        }
    }
}