namespace Terka.FontBuilder.Parser.Output
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Terka.FontBuilder.Testing;

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement
    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    /// <summary>
    /// Tests for the <see cref="RangeGlyphClassDefinition"/> class.
    /// </summary>
    [TestFixture]
    public class RangeGlyphClassDefinitionTests
    {
        /// <summary>
        /// Tests that ClassAssignments returns correct class assignments.
        /// </summary>
        [Test]
        public void ClassAssignments_TwoRanges_ReturnsCorrectClassAssignments()
        {
            var classDef = new RangeGlyphClassDefinition
            {
                ClassRanges = new Dictionary<Tuple<ushort, ushort>, ushort>
                {
                    { new Tuple<ushort, ushort>(2, 4), 3 },
                    { new Tuple<ushort, ushort>(6, 8), 4 },
                }
            };

            var expected = new IGrouping<ushort, ushort>[]
            {
                new GroupingStub<ushort, ushort>(3) { 2, 3, 4 }, 
                new GroupingStub<ushort, ushort>(4) { 6, 7, 8 }
            };

            Assert.That(expected, Is.EquivalentTo(classDef.ClassAssignments));
        }

        /// <summary>
        /// Tests that CreateEmptyClassDef returns empty class definitions.
        /// </summary>
        [Test]
        public void ClassAssignments_CreateEmptyClassDef_ReturnsEmptyClassAssignments()
        {
            Assert.IsEmpty(RangeGlyphClassDefinition.CreateEmptyClassDef().ClassAssignments);
        }
    }
}
