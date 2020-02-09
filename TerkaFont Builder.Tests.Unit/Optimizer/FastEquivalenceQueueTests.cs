namespace Terka.FontBuilder.Optimizer
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Terka.FontBuilder.Optimizer.Testing;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="FastEquivalenceQueue"/> class.
    /// </summary>
    [TestFixture]
    public class FastEquivalenceQueueTests
    {
        /// <summary>
        /// Tests that Equeue actually adds the item to the queue.
        /// </summary>
        [Test]
        public void Equeue_EqueueOne_Enqueues()
        {
            var queue = new FastEquivalenceQueue<int>(EqualityComparer<int>.Default);
            queue.Enqueue(1);

            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.AreEqual(1, result.Single());
        }

        /// <summary>
        /// Tests that Equeue doesn't enqueue one item more than once.
        /// </summary>
        [Test]
        public void DequeueEquivalenceSet_EqueueNonUniqueItem_EnqueuesOnlyOnce()
        {
            var queue = new FastEquivalenceQueue<int>(EqualityComparer<int>.Default);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(1);

            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Tests that first DequeueEquivalenceSet returns all the elements in the first equivalence class.
        /// </summary>
        [Test]
        public void DequeueEquivalenceSet_TwoEquivalenceClasses_ReturnsOnlyItemsFromFirstClass()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(2));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.That(result, Is.EquivalentTo(new[] {1, 3}));
        }

        /// <summary>
        /// Tests that second DequeueEquivalenceSet returns all the elements in the second equivalence class.
        /// </summary>
        [Test]
        public void DequeueEquivalenceSet_TwoEquivalenceClassesSecondDequeue_ReturnsOnlyItemsFromSecondClass()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(2));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            queue.DequeueEquivalenceSet();
            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.That(result, Is.EquivalentTo(new[] { 2, 4 }));
        }

        /// <summary>
        /// Tests that first DequeueEquivalenceSet returns all the elements in the first equivalence class, when there are three equivalence
        /// classes, but only two hash codes (therefore cónflict has to be resolved internally).
        /// </summary>
        [Test]
        public void DequeueEquivalenceSet_ThreeEquivalenceClassesWithTwoHashesFirstDequeue_ReturnsOnlyItemsFromFirstClass()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(3, 2));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);
            queue.Enqueue(5);
            queue.Enqueue(6);

            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.That(result, Is.EquivalentTo(new[] { 1, 4 }));
        }

        /// <summary>
        /// Tests that third DequeueEquivalenceSet returns all the elements in the first equivalence class, when there are three equivalence
        /// classes, but only two hash codes (therefore cónflict has to be resolved internally).
        /// </summary>
        [Test]
        public void DequeueEquivalenceSet_ThreeEquivalenceClassesWithTwoHashesThirdDequeue_ReturnsOnlyItemsFromThirdClass()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(3, 2));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);
            queue.Enqueue(5);
            queue.Enqueue(6);

            var a = queue.DequeueEquivalenceSet();
            var b = queue.DequeueEquivalenceSet();
            var result = queue.DequeueEquivalenceSet().ToList();

            Assert.That(result, Is.EquivalentTo(new[] { 3, 6 }));
        }

        /// <summary>
        /// Tests that IsEmpty returns true when the queue is empty.
        /// </summary>
        [Test]
        public void IsEmpty_EmptyQueue_ReturnsTrue()
        {
            Assert.IsTrue(new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(3, 2)).IsEmpty);
        }

        /// <summary>
        /// Tests that IsEmpty returns false when the queue is not empty.
        /// </summary>
        [Test]
        public void IsEmpty_NotEmptyQueue_ReturnsFalse()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(3, 2));
            queue.Enqueue(2);
            Assert.IsFalse(queue.IsEmpty);
        }

        /// <summary>
        /// Tests that Count returns total number of items among all the equivalency classes.
        /// </summary>
        [Test]
        public void Count_TwoEquivalenceClasses_ReturnsTotalNumberOfItems()
        {
            var queue = new FastEquivalenceQueue<int>(new ModuloIntegerEqualityComparer(2));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(3, queue.Count);
        }
    }
}