namespace Terka.FontBuilder.Simulator.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="LinkedListNodeExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class LinkedListNodeExtensionsTests
    {
        /// <summary>
        /// Returns linked list of integers from 0 to 9.
        /// </summary>
        /// <returns>The list.</returns>
        public LinkedList<int> GetList()
        {
            return new LinkedList<int>(Enumerable.Range(0, 10));
        }

        /// <summary>
        /// Tests that NextBy returns the same node if called with 0.
        /// </summary>
        [Test]
        public void NextBy_By0_ReturnsSameNode()
        {
            var list = this.GetList();
            var node = list.First;

            var result = node.NextBy(0);

            Assert.AreEqual(node, result);
        }

        /// <summary>
        /// Tests that NextBy returns the sixth node if called with five on the first node.
        /// </summary>
        [Test]
        public void NextBy_FromFirstBy5_ReturnsSixthNode()
        {
            var list = this.GetList();
            var node = list.First;

            var result = node.NextBy(5);

            Assert.AreEqual(5, result.Value);
        }

        /// <summary>
        /// Tests that NextBy returns null if called with too large number (which would end beyond the last node).
        /// </summary>
        [Test]
        public void NextBy_BeyondLastElement_ReturnsNull()
        {
            var list = this.GetList();
            var node = list.First;

            var result = node.NextBy(20);

            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// Tests that NextBy returns the sixth node if called with five on the first node.
        /// </summary>
        [Test]
        public void NextBy_FromEndByMinus5_ReturnsFifthNode()
        {
            var list = this.GetList();
            var node = list.Last;

            var result = node.NextBy(-5);

            Assert.AreEqual(4, result.Value);
        }

        /// <summary>
        /// Tests that NextBy returns null if called with -20 from the last node.
        /// </summary>
        [Test]
        public void NextBy_FromEndByNegativeBeyondFirstNode_ReturnsNull()
        {
            var list = this.GetList();
            var node = list.Last;

            var result = node.NextBy(-20);

            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// Tests that PreviousBy returns the same node if called with 0.
        /// </summary>
        [Test]
        public void PreviousBy_By0_ReturnsSameNode()
        {
            var list = this.GetList();
            var node = list.Last;

            var result = node.PreviousBy(0);

            Assert.AreEqual(node, result);
        }

        /// <summary>
        /// Tests that NextBy returns the sixth node if called with five on the first node.
        /// </summary>
        [Test]
        public void PreviousBy_FromLastBy5_ReturnsFifthNode()
        {
            var list = this.GetList();
            var node = list.Last;

            var result = node.PreviousBy(5);

            Assert.AreEqual(4, result.Value);
        }

        /// <summary>
        /// Tests that PreviousBy returns null if called with too large number (which would end beyond the last node).
        /// </summary>
        [Test]
        public void PreviousBy_BeyondFirstElement_ReturnsNull()
        {
            var list = this.GetList();
            var node = list.Last;

            var result = node.PreviousBy(20);

            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// Tests that PreviousBy returns the fifth node if called with five on the last node.
        /// </summary>
        [Test]
        public void PreviousBy_FromFirstByMinus5_ReturnsFifthNode()
        {
            var list = this.GetList();
            var node = list.First;

            var result = node.PreviousBy(-5);

            Assert.AreEqual(5, result.Value);
        }

        /// <summary>
        /// Tests that PreviousBy returns null if called with -20 from the first node.
        /// </summary>
        [Test]
        public void PreviousBy_FromFirstByNegativeBeyondLastNode_ReturnsNull()
        {
            var list = this.GetList();
            var node = list.First;

            var result = node.PreviousBy(-20);

            Assert.AreEqual(null, result);
        }
    }
}
