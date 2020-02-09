namespace Terka.FontBuilder.Parser.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;    

    /// <summary>  
    /// A 10 minute wrapper to access private members, havn't tested in detail.  
    /// Use under your own risk - amazedsaint@gmail.com  
    /// </summary>  
    public class AccessPrivateWrapper : DynamicObject
    {
        /// <summary>  
        /// Specify the Flags for accessing members  
        /// </summary>  
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Static | BindingFlags.Public;

        /// <summary>  
        /// The object we are going to wrap  
        /// </summary>  
        private readonly object wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessPrivateWrapper" /> class.
        /// </summary>
        /// <param name="o">The object to wrap.</param>
        public AccessPrivateWrapper(object o)
        {
            this.wrapped = o;
        }

        /// <summary>
        /// Gets the wrapped object.
        /// </summary>
        /// <value>
        /// The wrapped object.
        /// </value>
        public dynamic Wrapped
        {
            get
            {
                return this.wrapped;
            }
        }

        /// <summary>
        /// Create an instance via the constructor matching the args
        /// </summary>
        /// <param name="asm">The assembly.</param>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns>The instance.</returns>
        public static dynamic FromType(Assembly asm, string type, params object[] args)
        {
            var allt = asm.GetTypes();
            var t = allt.FirstOrDefault(item => item.Name == type);

            if (t == null)
            {
                throw new ArgumentException("Type does not exist.");
            }

            var types = from a in args
                        select a.GetType();

            // Gets the constructor matching the specified set of args  
            var ctor = t.GetConstructor(Flags, null, types.ToArray(), null);

            if (ctor == null)
            {
                throw new ArgumentException("Ctor does not exist.");
            }

            var instance = ctor.Invoke(args);
            return new AccessPrivateWrapper(instance);
        }

        /// <inheritdoc/>
        [DebuggerHidden]
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var types = from a in args select a.GetType();

            var method = this.wrapped.GetType().GetMethod(binder.Name, Flags, null, types.ToArray(), null);

            if (method == null)
            {
                return base.TryInvokeMember(binder, args, out result);
            }
            else
            {
                result = method.Invoke(this.wrapped, args);
                return true;
            }
        }

        /// <inheritdoc/>
        [DebuggerHidden]
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            // Try getting a property of that name  
            var prop = this.wrapped.GetType().GetProperty(binder.Name, Flags);

            if (prop == null)
            {
                // Try getting a field of that name  
                var fld = this.wrapped.GetType().GetField(binder.Name, Flags);
                if (fld != null)
                {
                    result = fld.GetValue(this.wrapped);
                    return true;
                }
                else
                {
                    return base.TryGetMember(binder, out result);
                }
            }
            else
            {
                result = prop.GetValue(this.wrapped, null);
                return true;
            }
        }

        /// <inheritdoc/>
        [DebuggerHidden]
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var prop = this.wrapped.GetType().GetProperty(binder.Name, Flags);
            if (prop == null)
            {
                var fld = this.wrapped.GetType().GetField(binder.Name, Flags);
                if (fld != null)
                {
                    fld.SetValue(this.wrapped, value);
                    return true;
                }
                else
                {
                    return base.TrySetMember(binder, value);
                }
            }
            else
            {
                prop.SetValue(this.wrapped, value, null);
                return true;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "Wrapped[" + this.Wrapped.GetType().Name + "]";
        }
    }
}
