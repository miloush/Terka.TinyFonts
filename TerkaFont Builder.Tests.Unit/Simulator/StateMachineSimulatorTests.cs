namespace Terka.FontBuilder.Simulator
{
    //using System.Collections.Generic;
    //using System.Linq;
    //using NUnit.Framework;
    //using Terka.FontBuilder.Extensions;
    //using Terka.FontBuilder.Compiler;
    //using Terka.FontBuilder.Compiler.Output;
    //using Terka.FontBuilder.Parser.Output;
    //using Terka.FontBuilder.Parser.Output.Context;
    //using Terka.FontBuilder.Parser.Output.Substitution;
    //using Terka.FontBuilder.Simulator.Extensions;

    //// ReSharper disable InconsistentNaming
    //// ReSharper disable ObjectCreationAsStatement
    //// ReSharper disable ReturnValueOfPureMethodIsNotUsed

    ///// <summary>
    ///// Tests for the <see cref="StateMachineSimulator"/> class.
    ///// </summary>
    //[TestFixture]
    //public class StateMachineSimulatorTests
    //{
    //    /// <summary>
    //    /// Creates the tape in glyoh linked list format from a collection of glyohIds.
    //    /// </summary>
    //    /// <param name="glyphIds">The glyph IDs.</param>
    //    /// <returns>The tape.</returns>
    //    public LinkedList<Glyph> CreateTapeList(IEnumerable<ushort> glyphIds)
    //    {
    //        return new LinkedList<Glyph>(glyphIds.Select(p => new Glyph { GlyphId = p }));
    //    }

    //    /// <summary>
    //    /// Compares the state of the tape (including position of its head) to an expected state.
    //    /// </summary>
    //    /// <param name="expectedTapeContent">Expected content of the tape.</param>
    //    /// <param name="expectedTapePosition">The expected tape position.</param>
    //    /// <param name="actualTapeContent">Actual content of the tape.</param>
    //    /// <param name="actualTapePosition">The actual tape position.</param>
    //    public void CompareTapeState(LinkedList<Glyph> expectedTapeContent, LinkedListNode<Glyph> expectedTapePosition, LinkedList<Glyph> actualTapeContent, LinkedListNode<Glyph> actualTapePosition)
    //    {
    //        Assert.That(actualTapeContent, Is.EquivalentTo(expectedTapeContent));
    //        Assert.That((expectedTapePosition == null) == (actualTapePosition == null));

    //        while (expectedTapePosition != null && actualTapePosition != null)
    //        {
    //            if ((expectedTapeContent.First == expectedTapePosition) != (actualTapeContent.First == actualTapePosition))
    //            {
    //                Assert.Fail("Tape positions are not equal.");
    //            }
                
    //            expectedTapePosition = expectedTapePosition.Previous;
    //            actualTapePosition = actualTapePosition.Previous;
    //        }
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a simple machine, which just lets it walk through the tape (with fallbacks to initial state).
    //    /// </summary>
    //    [Test]
    //    public void Simulate_WalkThroughStateMachine_LeavesStringUnchanged()
    //    {
    //        var tape = new ushort[] { 1, 2, 1, 2, 1, 2 };

    //        var machine = new StateMachine(
    //            new SubstitutionAction
    //            {
    //                HeadShift = 1,
    //                Transitions = new ITransition[]
    //                {
    //                    new SimpleTransition(
    //                        1, 
    //                        new SubstitutionAction
    //                        {
    //                            HeadShift = 1,
    //                            Transitions = new ITransition[]
    //                            {
    //                                new SimpleTransition(2, new SubstitutionAction { HeadShift = 1 })
    //                            }
    //                        })
    //                }
    //            });
            
    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = new[]
    //        {
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 2 }, 
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 2 }, 
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 2 }
    //        };

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly advances through completely unrecognizable sequence.
    //    /// </summary>
    //    [Test]
    //    public void Simulate_UnrecognizableSequence_WalksThrough()
    //    {
    //        var tape = new ushort[] { 3, 3, 3 };

    //        var machine = new StateMachine(
    //            new SubstitutionAction
    //            {
    //                HeadShift = 1,
    //                Transitions = new ITransition[]
    //                {
    //                    new SimpleTransition(
    //                        1, 
    //                        new SubstitutionAction
    //                        {
    //                            HeadShift = 1
    //                        })
    //                }
    //            });

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = new[]
    //        {
    //            new Glyph { GlyphId = 3 }, 
    //            new Glyph { GlyphId = 3 }, 
    //            new Glyph { GlyphId = 3 }
    //        };

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly recovers from fallback into an unrecognizable sequence.
    //    /// </summary>
    //    [Test]
    //    public void Simulate_UnrecognizableSequenceAfterRecognizableStart_WalksThrough()
    //    {
    //        var tape = new ushort[] { 1, 3, 3, 3 };

    //        var machine = new StateMachine(
    //            new SubstitutionAction
    //            {
    //                HeadShift = 1,
    //                Transitions = new ITransition[]
    //                {
    //                    new SimpleTransition(
    //                        1, 
    //                        new SubstitutionAction
    //                        {
    //                            HeadShift = 1
    //                        })
    //                }
    //            });

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = new[]
    //        {
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 3 }, 
    //            new Glyph { GlyphId = 3 },
    //            new Glyph { GlyphId = 3 }
    //        };

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a simple machine, which just lets it walk through the tape and replaces 2s with 3s.
    //    /// </summary>
    //    [Test]
    //    public void Simulate_SimpleReplacement_Replaces2sWith3s()
    //    {
    //        var tape = new ushort[] { 1, 2, 1, 2, 1, 2 };

    //        var machine = new StateMachine(
    //            new SubstitutionAction
    //            {
    //                HeadShift = 1,
    //                Transitions = new ITransition[]
    //                {
    //                    new SimpleTransition(
    //                        1, 
    //                        new SubstitutionAction
    //                        {
    //                            HeadShift = 1,
    //                            Transitions = new ITransition[]
    //                            {
    //                                new SimpleTransition(
    //                                    2, 
    //                                    new SubstitutionAction
    //                                    {
    //                                        HeadShift = 1,
    //                                        ReplacedGlyphCount = 1,
    //                                        ReplacementGlyphIds = new ushort[] { 3 }
    //                                    })
    //                            }
    //                        })
    //                }
    //            });

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = new[]
    //        {
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 3 }, 
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 3 }, 
    //            new Glyph { GlyphId = 1 }, 
    //            new Glyph { GlyphId = 3 }
    //        };

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a simple machine, which just lets it walk through the tape and replaces 2s with 3s.
    //    /// </summary>
    //    /// <param name="testId">The test ID. R# wouldn't be able to tell the two testcases apart otherwise (the parameter has no other purpose).</param>
    //    /// <param name="input">The input.</param>
    //    /// <param name="expectedGlyphIds">The expected glyph ids.</param>
    //    [TestCase(1, new ushort[] { 1, 1, 2, 4, 1, 1, 2, 1 }, new ushort[] { 1, 12, 4, 1, 12, 1 })]
    //    [TestCase(2, new ushort[] { 1, 1, 2, 3, 1, 1, 2, 3, 1 }, new ushort[] { 1, 123, 1, 123, 1 })]
    //    public void Simulate_LigatureReplacement_CorrectlyReplacesLigature(int testId, ushort[] input, ushort[] expectedGlyphIds)
    //    {
    //        var tape = input;

    //        /* Following ligatures are used in this test:
    //         * 1 2-> 12
    //         * 1 2 3 -> 12
    //         */
    //        var table = new LigatureSubstitutionTable
    //        {
    //            Coverage = new ListCoverageTable
    //            {
    //                CoveredGlyphIdList = new ushort[] { 1 }
    //            },
    //            LigatureSets = new[]
    //            {
    //                new[]
    //                {
    //                    new Ligature
    //                    {
    //                        ComponentGlyphIds = new ushort[] { 2 }, 
    //                        LigatureGlyphId = 12
    //                    }, 
    //                    new Ligature
    //                    {
    //                        ComponentGlyphIds = new ushort[] { 2, 3 }, 
    //                        LigatureGlyphId = 123
    //                    }
    //                }
    //            }
    //        };

    //        var builder = new StateMachineBuilder<SubstitutionAction>();

    //        var compiler = new SubstitutionCompiler();
    //        compiler.CompileTransformation(table, builder);

    //        var machine = builder.GetStateMachine();

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = expectedGlyphIds.Select(p => new Glyph { GlyphId = p });

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a simple machine, which just lets it walk through the tape and replaces 2s with 3s. It leaves 99s in middle of the ligatures
    //    /// untouched.
    //    /// </summary>
    //    /// <param name="testId">The test ID. R# wouldn't be able to tell the two testcases apart otherwise (the parameter has no other purpose).</param>
    //    /// <param name="input">The input.</param>
    //    /// <param name="expectedGlyphIds">The expected glyph ids.</param>
    //    [TestCase(1, new ushort[] { 1, 1, 99, 2, 4, 1, 1, 2, 1 }, new ushort[] { 1, 99, 12, 4, 1, 12, 1 })]
    //    [TestCase(2, new ushort[] { 1, 1, 99, 2, 3, 1, 1, 2, 3, 1 }, new ushort[] { 1, 99, 123, 1, 123, 1 })]
    //    public void Simulate_LigatureReplacementWithLookupFlags_CorrectlyReplacesLigature(int testId, ushort[] input, ushort[] expectedGlyphIds)
    //    {
    //        var tape = input;

    //        var metadata = new GlyphMetadata
    //        {
    //            GlyphIdToGlyphClassMapping = new Dictionary<ushort, GlyphClass>
    //            {
    //                { 99, GlyphClass.Mark },                    
    //            }.GetValueOrDefault
    //        };

    //        /* Following ligatures are used in this test:
    //         * 1 2-> 12
    //         * 1 2 3 -> 12
    //         */
    //        var table = new LigatureSubstitutionTable
    //        {
    //            Coverage = new ListCoverageTable
    //            {
    //                CoveredGlyphIdList = new ushort[] { 1 }
    //            },
    //            LookupFlags = LookupFlags.IgnoreMarks,
    //            LigatureSets = new[]
    //            {
    //                new[]
    //                {
    //                    new Ligature
    //                    {
    //                        ComponentGlyphIds = new ushort[] { 2 }, 
    //                        LigatureGlyphId = 12
    //                    }, 
    //                    new Ligature
    //                    {
    //                        ComponentGlyphIds = new ushort[] { 2, 3 }, 
    //                        LigatureGlyphId = 123
    //                    }
    //                }
    //            }
    //        };

    //        var builder = new StateMachineBuilder<SubstitutionAction>();

    //        var compiler = new SubstitutionCompiler();
    //        compiler.CompileTransformation(table, builder);

    //        var machine = builder.GetStateMachine();

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, metadata, tape);

    //        var expected = expectedGlyphIds.Select(p => new Glyph { GlyphId = p });

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a state machine, which replaces 2 in 1 2 3 with 4.
    //    /// </summary>
    //    [Test]
    //    public void Simulate_ContextReplacement_CorrectlyReplaces()
    //    {
    //        var tape = new ushort[] { 1, 2, 1, 2, 3, 2, 1, 2 };
    //        var expectedGlyphIds = new ushort[] { 1, 2, 1, 4, 3, 2, 1, 2 };            

    //        var table = new GlyphContextTransformationTable
    //        {
    //            Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 1 }) },
    //            TransformationRules = new[]
    //            {
    //                new[]
    //                {
    //                    new ContextTransformationRule
    //                    {
    //                        Context = new ushort[] { 2, 3 },
    //                        TransformationSets = new[]
    //                        {
    //                            new ContextTransformationSet
    //                            {
    //                                FirstGlyphIndex = 1,
    //                                Transformations = new[]
    //                                {
    //                                     new SimpleReplacementSubstitutionTable
    //                                     {
    //                                         Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 2 }) },
    //                                         ReplacementGlyphIds = new ushort[] { 4 }
    //                                     }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        };

    //        var builder = new StateMachineBuilder<SubstitutionAction>();

    //        var compiler = new SubstitutionCompiler();
    //        compiler.CompileTransformation(table, builder);

    //        var machine = builder.GetStateMachine();

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, new GlyphMetadata(), tape);

    //        var expected = expectedGlyphIds.Select(p => new Glyph { GlyphId = p });

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that Simulate correctly executes a state machine, which replaces 2 in 1 2 3 with 4 and completely ignores 99s classified as marks.
    //    /// </summary>
    //    [Test]
    //    public void Simulate_ContextReplacementWithLookupFlags_CorrectlyReplaces()
    //    {
    //        var tape = new ushort[] { 1, 2, 1, 99, 2, 99, 3, 2, 1, 2 };
    //        var expectedGlyphIds = new ushort[] { 1, 2, 1, 99, 4, 99, 3, 2, 1, 2 };

    //        var metadata = new GlyphMetadata
    //        {
    //            GlyphIdToGlyphClassMapping = new Dictionary<ushort, GlyphClass>
    //            {
    //                { 99, GlyphClass.Mark },                    
    //            }.GetValueOrDefault
    //        };

    //        var table = new GlyphContextTransformationTable
    //        {
    //            Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 1 }) },
    //            LookupFlags = LookupFlags.IgnoreMarks,
    //            TransformationRules = new[]
    //            {
    //                new[]
    //                {
    //                    new ContextTransformationRule
    //                    {
    //                        Context = new ushort[] { 2, 3 },
    //                        TransformationSets = new[]
    //                        {
    //                            new ContextTransformationSet
    //                            {
    //                                FirstGlyphIndex = 1,
    //                                Transformations = new[]
    //                                {
    //                                     new SimpleReplacementSubstitutionTable
    //                                     {
    //                                         Coverage = new ListCoverageTable { CoveredGlyphIdList = new HashSet<ushort>(new ushort[] { 2 }) },
    //                                         ReplacementGlyphIds = new ushort[] { 4 }
    //                                     }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        };

    //        var builder = new StateMachineBuilder<SubstitutionAction>();

    //        var compiler = new SubstitutionCompiler();
    //        compiler.CompileTransformation(table, builder);

    //        var machine = builder.GetStateMachine();

    //        var simulator = new StateMachineSimulator();
    //        var result = simulator.Simulate(machine, metadata, tape);

    //        var expected = expectedGlyphIds.Select(p => new Glyph { GlyphId = p });

    //        Assert.That(result, Is.EquivalentTo(expected));
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution doesn't alter the tape when the substitution state is not supposed to do so.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_ZeroSubstitutionAction_TapeStateNotChanged()
    //    {
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3 });
    //        var currentNode = tape.First.Next;
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 0,
    //            ReplacementGlyphIds = Enumerable.Empty<ushort>()
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, new GlyphMetadata(), ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 2, 3 });
    //        this.CompareTapeState(expectedTape, expectedTape.First.Next, tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph removal state removes the glyphs is supposed to and positions the 
    //    /// head to the state which precedes the removed segment.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_DeleteGlyphsFromTheMiddle_GlyphsRemovedAndHeadPositioned()
    //    {
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4, 5 });
    //        var currentNode = tape.First.NextBy(3);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 2,
    //            ReplacementGlyphIds = Enumerable.Empty<ushort>()
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, new GlyphMetadata(), ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 2, 5 });
    //        this.CompareTapeState(expectedTape, expectedTape.First.Next, tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph removal state removes the glyphs and positions the head on the first glyph if
    //    /// a prefix to the tape was deleted.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_DeleteGlyphsFromTheStart_GlyphsRemovedAndHeadPositioned()
    //    {
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4 });
    //        var currentNode = tape.First.NextBy(1);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 2,
    //            ReplacementGlyphIds = Enumerable.Empty<ushort>()
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, new GlyphMetadata(), ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 3, 4 });
    //        this.CompareTapeState(expectedTape, expectedTape.First, tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph insertion state inserts the glyphs and positions the head to the first
    //    /// glyph inserted.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_InsertGlyphs_GlyphsInsertedAndHeadPositioned()
    //    {
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 5, 6 });
    //        var currentNode = tape.First.NextBy(1);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 0,
    //            ReplacementGlyphIds = new ushort[] { 3, 4 }
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, new GlyphMetadata(), ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4, 5, 6 });
    //        this.CompareTapeState(expectedTape, expectedTape.First.NextBy(2), tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph insertion state inserts the glyphs and positions the head to the first
    //    /// glyph inserted.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_SubstituteGlyphs_GlyphsSubstitutedAndHeadPositioned()
    //    {
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4, 5 });
    //        var currentNode = tape.First.NextBy(3);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 3,
    //            ReplacementGlyphIds = new ushort[] { 12, 13, 14 }
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, new GlyphMetadata(), ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 12, 13, 14, 5 });
    //        this.CompareTapeState(expectedTape, expectedTape.First.Next, tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph removal state removes the glyphs is supposed to and positions the 
    //    /// head to the state which precedes the removed segment.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_DeleteGlyphsFromTheMiddleWithLookupFlags_IgnoresIgnoredGlyphs()
    //    {
    //        var metadata = new GlyphMetadata()
    //        {
    //            GlyphIdToGlyphClassMapping = new Dictionary<ushort, GlyphClass>
    //            {
    //                { 3, GlyphClass.Ligature }                                         
    //            }.GetValueOrDefault                   
    //        };
            
    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4, 5, 6 });
    //        var currentNode = tape.First.NextBy(4);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 3,
    //            ReplacementGlyphIds = Enumerable.Empty<ushort>(),
    //            LookupFlags = LookupFlags.IgnoreLigatures
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, metadata, ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 3, 6 });
    //        this.CompareTapeState(expectedTape, expectedTape.First, tape, currentNode);
    //    }

    //    /// <summary>
    //    /// Tests that DoSubstitution with according glyph insertion state inserts the glyphs and positions the head to the first
    //    /// glyph inserted. The glyphs are inserted after the last ignored glyph.
    //    /// </summary>
    //    [Test]
    //    public void DoSubstitution_SubstituteGlyphsWithLookupFlags_InsertsAfterLastIgnoredGlyph()
    //    {
    //        var metadata = new GlyphMetadata()
    //        {
    //            GlyphIdToGlyphClassMapping = new Dictionary<ushort, GlyphClass>
    //            {
    //                { 3, GlyphClass.Ligature },
    //                { 5, GlyphClass.Ligature }                          
    //            }.GetValueOrDefault
    //        };

    //        var tape = this.CreateTapeList(new ushort[] { 1, 2, 3, 4, 5, 6, 7 });
    //        var currentNode = tape.First.NextBy(5);
    //        var state = new SubstitutionAction
    //        {
    //            ReplacedGlyphCount = 3,
    //            ReplacementGlyphIds = new ushort[] { 12, 13, 14 },
    //            LookupFlags = LookupFlags.IgnoreLigatures
    //        };

    //        var simulator = new StateMachineSimulator();
    //        simulator.DoSubstitution(tape, metadata, ref currentNode, state);

    //        var expectedTape = this.CreateTapeList(new ushort[] { 1, 3, 5, 12, 13, 14, 7 });
    //        this.CompareTapeState(expectedTape, expectedTape.First.NextBy(3), tape, currentNode);
    //    }
    //}
}
