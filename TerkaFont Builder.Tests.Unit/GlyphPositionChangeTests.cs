namespace Terka.FontBuilder
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    #pragma warning disable 252,253 // Unwanted assignment

    /// <summary>
    /// Tests for the <see cref="GlyphPositionChange"/> class.
    /// </summary>
    [TestFixture]
    public class GlyphPositionChangeTests
    {
        /// <summary>
        /// Tests that Equals returns true on two equal instances.
        /// </summary>
        [Test]
        public void Equals_EqualInstances_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance is null.
        /// </summary>
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a.Equals(null));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance is of different type.
        /// </summary>
        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a.Equals(new object()));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance has different advanceX.
        /// </summary>
        [Test]
        public void Equals_DifferentAdvanceX_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 0,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance has different advanceY.
        /// </summary>
        [Test]
        public void Equals_DifferentAdvanceY_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 0,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance has different offsetX.
        /// </summary>
        [Test]
        public void Equals_DifferentOffsetX_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 0,
                OffsetY = 4
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false when the other instance has different offsetY.
        /// </summary>
        [Test]
        public void Equals_DifferentOffsetY_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 0
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that == returns true on two equal instances.
        /// </summary>
        [Test]
        public void EqualsOperator_EqualInstances_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a == b);
        }

        /// <summary>
        /// Tests that == returns false when the other instance is null.
        /// </summary>
        [Test]
        public void EqualsOperator_Null_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a == null);
        }

        /// <summary>
        /// Tests that == returns false when the other instance is of different type.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentType_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a == new object());
        }

        /// <summary>
        /// Tests that == returns false when the other instance has different advanceX.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentAdvanceX_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 0,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false when the other instance has different advanceY.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentAdvanceY_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 0,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false when the other instance has different offsetX.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentOffsetX_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 0,
                OffsetY = 4
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false when the other instance has different offsetY.
        /// </summary>
        [Test]
        public void EqualsOperator_DifferentOffsetY_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 0
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that != returns true on two equal instances.
        /// </summary>
        [Test]
        public void NotEqualsOperator_EqualInstances_ReturnsFalse()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsFalse(a != b);
        }

        /// <summary>
        /// Tests that != returns false when the other instance is null.
        /// </summary>
        [Test]
        public void NotEqualsOperator_Null_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a != null);
        }

        /// <summary>
        /// Tests that != returns false when the other instance is of different type.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentType_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a != new object());
        }

        /// <summary>
        /// Tests that != returns false when the other instance has different advanceX.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentAdvanceX_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 0,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns false when the other instance has different advanceY.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentAdvanceY_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 0,
                OffsetX = 3,
                OffsetY = 4
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns false when the other instance has different offsetX.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentOffsetX_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 0,
                OffsetY = 4
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns false when the other instance has different offsetY.
        /// </summary>
        [Test]
        public void NotEqualsOperator_DifferentOffsetY_ReturnsTrue()
        {
            var a = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 4
            };

            var b = new GlyphPositionChange
            {
                AdvanceX = 1,
                AdvanceY = 2,
                OffsetX = 3,
                OffsetY = 0
            };

            Assert.IsTrue(a != b);
        }
    }
}
