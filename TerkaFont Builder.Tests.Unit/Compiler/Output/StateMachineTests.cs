namespace Terka.FontBuilder.Compiler.Output
{
    using System;

    using NUnit.Framework;

    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Substitution;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests for the <see cref="StateMachine"/> class.
    /// </summary>
    [TestFixture]
    public class StateMachineTests
    {
        /// <summary>
        /// Tests that GetGeneratedGlyphIds generates collection of all generated glyph IDs as expected.
        /// </summary>
        [Test]
        public void GetGeneratedGlyphIds_StateMachone_GeneratesCorrectCollection()
        {
            var table = new MultipleSubstitutionTable
            {
                Coverage = new ListCoverageTable
                {
                    CoveredGlyphIdList = new ushort[] { 1, 2 }
                },
                ReplacementSequences = new[]
                {
                    new ushort[] { 3, 1, 2 }, 
                    new ushort[] { 4, 3, 2 }
                }
            };

            var compiler = new SubstitutionCompiler();
            var machine = compiler.Compile(new[] { table });

            Assert.That(machine.GetGeneratedGlyphIds(), Is.EquivalentTo(new ushort[] { 1, 2, 3, 4 }));
        }
 
    }
}