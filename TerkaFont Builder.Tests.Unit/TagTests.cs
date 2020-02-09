namespace Terka.FontBuilder
{
    using System;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable EqualExpressionComparison
    // ReSharper disable ObjectCreationAsStatement
    #pragma warning disable 252,253 // Possible reference comparison

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="Tag"/> class.
    /// </summary>
    [TestFixture]
    public class TagTests
    {
        /// <summary>
        /// Tests that Ctor throws exception when called with label of different length than 4.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_BadLengthLabel_ThrowsException()
        {
            new Tag("abcdefg");
        }

        /// <summary>
        /// Tests that Ctor throws exception when called with null label.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullLabel_ThrowsException()
        {
            new Tag(null);
        }

        /// <summary>
        /// Tests that == returns false when a tag is compared with tag with different label.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentLabelTag_ReturnFalse()
        {
            Assert.IsFalse(new Tag("abcd") == new Tag("defg"));
        }

        /// <summary>
        /// Tests that == returns true when a tag is compared with equal tag.
        /// </summary>
        [Test]
        public void EqualsOperator_EqualLabelTag_ReturnTrue()
        {
            Assert.IsTrue(new Tag("abcd") == new Tag("abcd"));
        }

        /// <summary>
        /// Tests that == returns false when a tag is compared with different type.
        /// </summary>
        [Test]
        public void EqualsOperator_NonTag_ReturnFalse()
        {
            Assert.IsFalse(new Tag("abcd") == new object());
        }

        /// <summary>
        /// Tests that == returns false when a tag is compared with null.
        /// </summary>
        [Test]
        public void EqualsOperator_Null_ReturnFalse()
        {
            Assert.IsFalse(new Tag("abcd") == null);
        }

        /// <summary>
        /// Tests that Equals returns false when compared with tag with different label.
        /// </summary>
        [Test]
        public void Equals_DifferentLabelTag_ReturnsFalse()
        {
            Assert.IsFalse(new Tag("abcd").Equals(new Tag("defg")));
        }

        /// <summary>
        /// Tests that Equals returns true when compared with null.
        /// </summary>
        [Test]
        public void Equals_EqualLabelTag_ReturnsTrue()
        {
            Assert.IsTrue(new Tag("abcd").Equals(new Tag("abcd")));
        }

        /// <summary>
        /// Tests that Equals returns false when compared with non-tag.
        /// </summary>
        [Test]
        public void Equals_NonTag_ReturnsFalse()
        {
            Assert.IsFalse(new Tag("abcd").Equals(new object()));
        }

        /// <summary>
        /// Tests that Equals returns false when compared with null.
        /// </summary>
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            Assert.IsFalse(new Tag("abcd").Equals(null));
        }

        /// <summary>
        /// Tests that GetHashCode returns hash of the label.
        /// </summary>
        [Test]
        public void GetHashCode_ValidLabel_ReturnsHashOfLabel()
        {
            Assert.AreEqual("abcd".GetHashCode(), new Tag("abcd").GetHashCode());
        }

        /// <summary>
        /// Tests that != returns true when a tag is compared with tag with different label.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentLabelTag_ReturnFalse()
        {
            Assert.IsTrue(new Tag("abcd") != new Tag("defg"));
        }

        /// <summary>
        /// Tests that != returns false when a tag is compared with equal tag.
        /// </summary>
        [Test]
        public void NotEqualsOperator_EqualLabelTag_ReturnFalse()
        {
            Assert.IsFalse(new Tag("abcd") != new Tag("abcd"));
        }

        /// <summary>
        /// Tests that != returns true when a tag is compared with different type.
        /// </summary>
        [Test]
        public void NotEqualsOperator_NonTag_ReturnFalse()
        {
            Assert.IsTrue(new Tag("abcd") != new object());
        }

        /// <summary>
        /// Tests that != returns true when a tag is compared with null.
        /// </summary>
        [Test]
        public void NotEqualsOperator_Null_ReturnFalse()
        {
            Assert.IsTrue(new Tag("abcd") != null);
        }
    }
}