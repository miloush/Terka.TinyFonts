namespace Terka.FontBuilder
{
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="GlyphMetadata"/> class.
    /// </summary>
    [TestFixture]
    public class GlyphMetadataTests
    {
        /// <summary>
        /// Tests that Default always returns empty delegates.
        /// </summary>
        [Test]
        public void Ctor_Always_ReturnsEmptyDelegates()
        {
            var m = new GlyphMetadata();
            
            Assert.AreEqual(63, m.CharacterToGlyphIdMapping((char)63));
            Assert.AreEqual(GlyphClass.Unknown, m.GlyphIdToGlyphClassMapping(63));
            Assert.AreEqual(0, m.GlyphIdToMarkAttachClassIdMapping(63));
        }
    }
}