namespace Terka.FontBuilder
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="Glyph"/> class.
    /// </summary>
    [TestFixture]
    public class GlyphTests
    {
        /// <summary>
        /// Tests that == returns true on equal glyphs.
        /// </summary>
        [Test]
        public void EqualityOperator_EqualGlyphs_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a == b);
        }

        /// <summary>
        /// Tests that == returns false on glyphs with different AdvanceX.
        /// </summary>
        [Test]
        public void EqualityOperator_DifferentAdvanceX_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 0,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a == b);
        }

                /// <summary>
        /// Tests that == returns false on glyphs with different AdvanceY.
        /// </summary>
        [Test]
        public void EqualityOperator_DifferentAdvanceY_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 0,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on glyphs with different GlyphId.
        /// </summary>
        [Test]
        public void EqualityOperator_DifferentGlyphId_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 0,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on glyphs with different OffsetX.
        /// </summary>
        [Test]
        public void EqualityOperator_DifferentOffsetX_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 0,
                OffsetY = 5
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on glyphs with different OffsetY.
        /// </summary>
        [Test]
        public void EqualityOperator_DifferentOffsetY_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 0
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that == returns false on null on right side of the comparison.
        /// </summary>
        [Test]
        public void EqualityOperator_RightSideNull_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a == null);
        }

        /// <summary>
        /// Tests that == returns false on null on left side of the comparison.
        /// </summary>
        [Test]
        public void EqualityOperator_LeftSideNull_ReturnsFalse()
        {
            Glyph a = null;
            
            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests that != returns false on equal glyphs.
        /// </summary>
        [Test]
        public void NonEqualityOperator_EqualGlyphs_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a != b);
        }

        /// <summary>
        /// Tests that != returns true on glyphs with different AdvanceX.
        /// </summary>
        [Test]
        public void NonEqualityOperator_DifferentAdvanceX_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 0,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on glyphs with different AdvanceY.
        /// </summary>
        [Test]
        public void NonEqualityOperator_DifferentAdvanceY_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 0,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on glyphs with different GlyphId.
        /// </summary>
        [Test]
        public void NonEqualityOperator_DifferentGlyphId_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 0,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on glyphs with different OffsetX.
        /// </summary>
        [Test]
        public void NonEqualityOperator_DifferentOffsetX_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 0,
                OffsetY = 5
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on glyphs with different OffsetY.
        /// </summary>
        [Test]
        public void NonEqualityOperator_DifferentOffsetY_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 0
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that != returns true on null on right side of the comparison.
        /// </summary>
        [Test]
        public void NonEqualityOperator_RightSideNull_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a != null);
        }

        /// <summary>
        /// Tests that != returns true on null on left side of the comparison.
        /// </summary>
        [Test]
        public void NonEqualityOperator_LeftSideNull_ReturnsFalse()
        {
            Glyph a = null;

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Tests that Equals returns true on equal glyph.
        /// </summary>
        [Test]
        public void Equals_EqualGlyph_ReturnsTrue()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on glyph with different AdvanceX.
        /// </summary>
        [Test]
        public void Equals_DifferentAdvanceX_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 0,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on glyph with different AdvanceY.
        /// </summary>
        [Test]
        public void Equals_DifferentAdvanceY_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 0,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on glyph with different GlyphId.
        /// </summary>
        [Test]
        public void Equals_DifferentGlyphId_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 0,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on glyph with different OffsetX.
        /// </summary>
        [Test]
        public void Equals_DifferentOffsetX_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 0,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on glyph with different OffsetY.
        /// </summary>
        [Test]
        public void Equals_DifferentOffsetY_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            var b = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 0
            };

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Tests that Equals returns false on null.
        /// </summary>
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(null));
        }

        /// <summary>
        /// Tests that Equals returns false on object of different type.
        /// </summary>
        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Glyph
            {
                AdvanceX = 1,
                AdvanceY = 2,
                GlyphId = 3,
                OffsetX = 4,
                OffsetY = 5
            };

            Assert.IsFalse(a.Equals(new object()));
        }
    }
}
