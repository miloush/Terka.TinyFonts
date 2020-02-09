namespace Terka.FontBuilder.Parser.Reflection
{
    using System;
    using System.Reflection;

    using Microsoft.CSharp.RuntimeBinder;

    using NUnit.Framework;

    using Terka.FontBuilder.Parser.Reflection.Testing;

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local
    #pragma warning disable 168

    /// <summary>
    /// Tests for the <see cref="AccessPrivateWrapper"/> class.
    /// </summary>
    [TestFixture]
    public class AccessPrivateWrapperTests
    {
        /// <summary>
        /// Tests that FromType creates an instance using private ctor.
        /// </summary>
        [Test]
        public void FromType_PrivateCtor_CreatesInstance()
        {
            dynamic o = AccessPrivateWrapper.FromType(Assembly.GetExecutingAssembly(), "PrivateAccessTester", 333);

            Assert.IsTrue(o.Wrapped.PrivateCtorCalled);
        }

        /// <summary>
        /// Tests that FromType creates an instance using public ctor.
        /// </summary>
        [Test]
        public void FromType_PublicCtor_CreatesInstance()
        {
            dynamic o = AccessPrivateWrapper.FromType(Assembly.GetExecutingAssembly(), "PrivateAccessTester");

            Assert.IsTrue(o.Wrapped.PublicCtorCalled);
        }

        /// <summary>
        /// Tests that FromType throws exception if the ctor does not exist.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FromType_UnknownCtor_ThrowsException()
        {
            AccessPrivateWrapper.FromType(Assembly.GetExecutingAssembly(), "PrivateAccessTester", "TheCtorDoesNotAcceptStrings");
        }

        /// <summary>
        /// Tests that FromType throws exception if the type does not exist.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FromType_UnknownType_ReturnsNull()
        {
            AccessPrivateWrapper.FromType(Assembly.GetExecutingAssembly(), "UnknownType");
        }

        /// <summary>
        /// Tests that TryInvokeMember invokes correct private function.
        /// </summary>
        [Test]
        public void TryInvokeMember_PrivateMethod_InvokesTheMethod()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            int result = o.PrivateMethod(555);

            Assert.IsTrue(o.PrivateMethodCalled);
            Assert.AreEqual(result, 444);
        }

        /// <summary>
        /// Tests that TryInvokeMember invokes correct public function.
        /// </summary>
        [Test]
        public void TryInvokeMember_PublicMethod_InvokesTheMethod()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            int result = o.PublicMethod(555);

            Assert.IsTrue(o.PublicMethodCalled);
            Assert.AreEqual(result, 666);
        }

        /// <summary>
        /// Tests that TryInvokeMember throws exception when invoking unknown method.
        /// </summary>
        [Test]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TryInvokeMember_UnknownMethod_ThrowsException()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            o.UnknownMethod(555);
        }

        /// <summary>
        /// Tests that TryGetMember correctly reads private field.
        /// </summary>
        [Test]
        public void TryGetMember_PrivateField_ReturnsCorrectValue()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            Assert.AreEqual(111, o.privateField);
        }

        /// <summary>
        /// Tests that TryGetMember correctly reads public field.
        /// </summary>
        [Test]
        public void TryGetMember_PublicField_ReturnsCorrectValue()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            Assert.AreEqual(222, o.PublicField);
        }

        /// <summary>
        /// Tests that TryGetMember throws exception when reading unknown field.
        /// </summary>
        [Test]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TryGetMember_UnknownField_ThrowsException()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            var x = o.UnknownField;
        }

        /// <summary>
        /// Tests that TryGetMember correctly writes private field.
        /// </summary>
        [Test]
        public void TrySetMember_PrivateField_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            o.privateField = 777;

            Assert.AreEqual(777, tester.PrivateFieldAccessor);
        }

        /// <summary>
        /// Tests that TryGetMember correctly writes private field.
        /// </summary>
        [Test]
        public void TrySetMember_PublicField_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            o.PublicField = 888;

            Assert.AreEqual(888, tester.PublicField);
        }

        /// <summary>
        /// Tests that TryGetMember throws exception when writing unknown field.
        /// </summary>
        [Test]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TrySetMember_UnknownField_ThrowsException()
        {
            dynamic o = new AccessPrivateWrapper(new PrivateAccessTester());

            o.UnknownField = 123;
        }

        /// <summary>
        /// Tests that TryGetMember correctly reads private field.
        /// </summary>
        [Test]
        public void TryGetMember_PrivateProperty_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            tester.PrivateGetProperty = 888;

            Assert.AreEqual(888, o.PrivateGetProperty);
        }

        /// <summary>
        /// Tests that TryGetMember correctly reads public field.
        /// </summary>
        [Test]
        public void TryGetMember_PublicProperty_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            tester.PublicProperty = 999;

            Assert.AreEqual(999, o.PublicProperty);
        }

        /// <summary>
        /// Tests that TryGetMember correctly writes private field.
        /// </summary>
        [Test]
        public void TrySetMember_PrivateProperty_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            o.PrivateSetProperty = 101010;

            Assert.AreEqual(101010, tester.PrivateSetProperty);
        }

        /// <summary>
        /// Tests that TryGetMember correctly writes private field.
        /// </summary>
        [Test]
        public void TrySetMember_PublicProperty_ReturnsCorrectValue()
        {
            var tester = new PrivateAccessTester();
            dynamic o = new AccessPrivateWrapper(tester);

            o.PublicProperty = 111111;

            Assert.AreEqual(111111, tester.PublicProperty);
        }
    }
}