namespace Terka.FontBuilder.Parser.Reflection.Testing
{
    // ReSharper disable ConvertToConstant.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "OK for tests.")]
    public class PrivateAccessTester
    {
        public bool PrivateMethodCalled = false;
        public bool PublicMethodCalled = false;

        public bool PrivateCtorCalled = false;
        public bool PublicCtorCalled = false;
        
        public int PublicField = 222;

        private int privateField = 111;

        public PrivateAccessTester()
        {
            this.PublicCtorCalled = true;
        }

        private PrivateAccessTester(int val)
        {
            this.PrivateCtorCalled = true;
        }

        public int PrivateGetProperty { private get; set; }

        public int PrivateSetProperty { get; private set; }

        public int PublicProperty { get; set; }

        public int PrivateFieldAccessor
        {
            get
            {
                return this.privateField;
            }
        }

        private int PrivateMethod(int arg)
        {
            this.PrivateMethodCalled = true;
            return 444;
        }

        private int PublicMethod(int arg)
        {
            this.PublicMethodCalled = true;
            return 666;
        }
    }
}