namespace Terka.FontBuilder.Extensions
{
    using System;
    using System.Linq;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable InvokeAsExtensionMethod
    // ReSharper disable RedundantCast

    /// <summary>
    /// Tests for the <see cref="EnumerableExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        /// <summary>
        /// Tests that Append throws a correct exception when called with null appendee.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_NullAppendee_ThrowsException()
        {
            new[] { new object() }.Append((object)null).ToList();
        }

        /// <summary>
        /// Tests that Append throws a correct exception when attemption to append a null collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_NullCollection_ThrowsException()
        {
            new[] { new object() }.Append((object[])null).ToList();
        }

        /// <summary>
        /// Tests that Append throws a correct exception when called on null collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_ToNullCollection_ThrowsException()
        {
            EnumerableExtensions.Append(null, 1).ToList();
        }

        /// <summary>
        /// Tests that collection Append throws a correct exception when called on null collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_CollectionToNullCollection_ThrowsException()
        {
            EnumerableExtensions.Append((int[])null, new[] { 1 }).ToList();
        }

        /// <summary>
        /// Tests that Append correctly appends the appended item in a normal scenario.
        /// </summary>
        [Test]
        public void Append_ValidCollectionAndItem_Appends()
        {
            var collection = new[] { 1, 2, 3, 4 };

            var result = collection.Append(5);

            Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EquivalentTo(result));
        }

        /// <summary>
        /// Tests that Append correctly appends two collections.
        /// </summary>
        [Test]
        public void Append_TwoValidCollections_Appends()
        {
            var collection = new[] { 1, 2, 3 };
            var collection2 = new[] { 4, 5, 6 };

            var result = collection.Append(collection2);

            Assert.That(new[] { 1, 2, 3, 4, 5, 6 }, Is.EquivalentTo(result));
        }

        /// <summary>
        /// Tests that Prepend throws a correct exception when called with null prependee.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prepend_NullAppendee_ThrowsException()
        {
            new[] { new object() }.Prepend(null).ToList();
        }

        /// <summary>
        /// Tests that Prepend throws a correct exception when attempting to append a null prependee collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prepend_NullCollection_ThrowsException()
        {
            new[] { new object() }.Prepend((object[])null).ToList();
        }

        /// <summary>
        /// Tests that Prepend throws a correct exception when called on null collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prepend_ToNullCollection_ThrowsException()
        {
            EnumerableExtensions.Prepend(null, 1).ToList();
        }

        /// <summary>
        /// Tests that Prepend correctly prepends the appended item in a normal scenario.
        /// </summary>
        [Test]
        public void Prepend_ValidCollectionAndItem_Prepends()
        {
            var collection = new[] { 1, 2, 3, 4 };

            var result = collection.Prepend(0);

            Assert.That(new[] { 0, 1, 2, 3, 4 }, Is.EquivalentTo(result));
        }

        /// <summary>
        /// Tests that ValuesEqual returns false on two collections of different lengths (but same values in the common part).
        /// </summary>
        [Test]
        public void ValuesEqual_DifferentCollectionLengths_ReturnsFalse()
        {
            var collection1 = new[] { 1, 2, 3 };
            var collection2 = new[] { 1, 2, 3, 4 };

            var result = collection1.ValuesEqual(collection2);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that ValuesEqual returns false on two collections of different values but the same lengths.
        /// </summary>
        [Test]
        public void ValuesEqual_DifferentCollectionValues_ReturnsFalse()
        {
            var collection1 = new[] { 1, 2, 3 };
            var collection2 = new[] { 4, 5, 6 };

            var result = collection1.ValuesEqual(collection2);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that ValuesEqual returns false on two collections, which would be different and even custom comparer won't save the day.
        /// </summary>
        [Test]
        public void ValuesEqual_DifferentCollectionsWithCustomComparer_ReturnsFalse()
        {
            var collection1 = new[] { "ab", "cd", "ef" };
            var collection2 = new[] { "CD", "EF", "GH" };

            var result = collection1.ValuesEqual(collection2, StringComparer.InvariantCultureIgnoreCase);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that ValuesEqual returns true on two collections, which would be different, but custom comparer makes them equal.
        /// </summary>
        [Test]
        public void ValuesEqual_EqualCollectionsWithCustomComparer_ReturnsTrue()
        {
            var collection1 = new[] { "ab", "cd", "ef" };
            var collection2 = new[] { "AB", "CD", "EF" };

            var result = collection1.ValuesEqual(collection2, StringComparer.InvariantCultureIgnoreCase);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that ValuesEqual returns true on two collection of different types, but with the same values.
        /// </summary>
        [Test]
        public void ValuesEqual_EqualCollections_ReturnsTrue()
        {
            var collection = new[] { 1, 2, 3, 4 };

            var result = collection.ValuesEqual(collection.ToList());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that ValuesEqual throws exception when the first collection is null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValuesEqual_NullFirstCollection_ThrowsException()
        {
            EnumerableExtensions.ValuesEqual(null, Enumerable.Empty<object>());
        }

        /// <summary>
        /// Tests that ValuesEqual throws exception when the second collection is null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValuesEqual_NullSecondCollection_ThrowsException()
        {
            Enumerable.Empty<object>().ValuesEqual(null);
        }

        /// <summary>
        /// Tests that Zip zips correctly.
        /// </summary>
        [Test]
        public void Zip_TwoCollections_Zips()
        {
            var a = new[] { 1, 2, 3 };
            var b = new[] { 'a', 'b', 'c' };

            var result = a.Zip(b);

            var expected = new[]
            {
                new Tuple<int, char>(1, 'a'),
                new Tuple<int, char>(2, 'b'),
                new Tuple<int, char>(3, 'c')
            };

            Assert.IsTrue(result.ValuesEqual(expected));
        }
    }
}