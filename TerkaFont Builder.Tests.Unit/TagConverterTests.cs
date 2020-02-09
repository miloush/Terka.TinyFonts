namespace Terka.FontBuilder
{
    using System;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="TagConverter"/> class.
    /// </summary>
    [TestFixture]
    public class TagConverterTests
    {
        /// <summary>
        /// Tests that TagFromUint encodes uint into a correct tag.
        /// </summary>
        [Test]
        public void TagFromUint_ValidUint_ReturnCorrectTag()
        {
            Tag result = TagConverter.TagFromUint(0x61626364);

            Assert.AreEqual("abcd", result.Label);
        }

        /// <summary>
        /// Tests that UintFromTag throws exception when fed with null tag.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UintFromTag_NullTag_ThrowsException()
        {
            TagConverter.UintFromTag(null);
        }

        /// <summary>
        /// Tests that UintFromTag correctly encodes a valid tag into an uint.
        /// </summary>
        [Test]
        public void UintFromTag_ValidTag_ReturnsCorrectUint()
        {
            uint result = TagConverter.UintFromTag(new Tag("abcd"));

            Assert.AreEqual(0x61626364, result);
        }
    }
}