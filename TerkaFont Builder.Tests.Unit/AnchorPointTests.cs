namespace Terka.FontBuilder
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="AnchorPoint"/> class.
    /// </summary>
    [TestFixture]
    public class AnchorPointTests
    {
        /// <summary>
        /// Tests that == returns true on two equal anchor points.
        /// </summary>
        [Test]
        public void EqualsOperator_EqualAnchorPoints_ReturnsTrue()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsTrue(a == b);
        }

        /// <summary>
        /// Tests that == returns false when null is on the right.
        /// </summary>
        [Test]
        public void EqualsOperator_RightSideNull_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsFalse(a == null);
        }

        /// <summary>
        /// Tests that == returns false when null is on the left.
        /// </summary>
        [Test]
        public void EqualsOperator_LeftSideNull_ReturnsFalse()
        {
            AnchorPoint a = null;
            var b = new AnchorPoint { X = 1, Y = 2 };            

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on different X.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentX_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 2, Y = 2 };  

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on different Y.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentY_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 1 };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that != returns false on two equal anchor points.
        /// </summary>
        [Test]
        public void NotEqualsOperator_EqualAnchorPoints_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsFalse(a != b);
        }

        /// <summary>
        /// Tests that != returns true when null is on the right.
        /// </summary>
        [Test]
        public void NotEqualsOperator_RightSideNull_ReturnsTrue()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsTrue(a != null);
        }

        /// <summary>
        /// Tests that != returns true when null is on the left.
        /// </summary>
        [Test]
        public void NotEqualsOperator_LeftSideNull_ReturnsTrue()
        {
            AnchorPoint a = null;
            var b = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on different X.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentX_ReturnsTrue()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 2, Y = 2 };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on different Y.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentY_ReturnsTrue()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 1 };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that Equals returns true on equal anchor point.
        /// </summary>
        [Test]
        public void Equals_EqualAnchorPoints_ReturnsTrue()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on null.
        /// </summary>
        [Test]
        public void Equals_RightSideNull_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsFalse(a.Equals(null));
        }

        /// <summary>
        /// Tests that Equals returns on different type.
        /// </summary>
        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };

            Assert.IsFalse(a.Equals(new object()));
        }

        /// <summary>
        /// Tests that Equals returns false on different X.
        /// </summary>
        [Test]
        public void Equals_DifferentX_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 2, Y = 2 };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on different Y.
        /// </summary>
        [Test]
        public void Equals_DifferentY_ReturnsFalse()
        {
            var a = new AnchorPoint { X = 1, Y = 2 };
            var b = new AnchorPoint { X = 1, Y = 1 };

            Assert.IsFalse(a.Equals(b));
        }
    }
}