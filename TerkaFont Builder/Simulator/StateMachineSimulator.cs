namespace Terka.FontBuilder.Simulator
{
    //using System;
    //using System.Collections.Generic;
    //using System.Globalization;
    //using System.Linq;

    //using Terka.FontBuilder.Compiler.Output;
    //using Terka.FontBuilder.Extensions;
    //using Terka.FontBuilder.Simulator.Extensions;

    ///// <summary>
    ///// Simulates execution of a font state machine. This execution scheme should be identical to the execution scheme of the same state machine after being serialized and loaded in the MF part.
    ///// </summary>
    //public class StateMachineSimulator
    //{
    //    /// <summary>
    //    /// Simulates the specified machine on a tape generated from a string.
    //    /// </summary>
    //    /// <param name="machine">The machine.</param>
    //    /// <param name="metadata">The glyph metadata.</param>
    //    /// <param name="inputString">The input string.</param>
    //    /// <returns>The resulting tape.</returns>
    //    public IEnumerable<Glyph> Simulate(StateMachine machine, GlyphMetadata metadata, string inputString)
    //    {
    //        return this.Simulate(machine, metadata, inputString.Select(metadata.CharacterToGlyphIdMapping));
    //    }

    //    /// <summary>
    //    /// Simulates the specified machine on a tape generated from a sequence of glyph IDs.
    //    /// </summary>
    //    /// <param name="machine">The machine.</param>
    //    /// <param name="metadata">The glyph metadata.</param>
    //    /// <param name="inputGlyphIds">The input glyph ID sequence.</param>
    //    /// <returns>The resulting tape.</returns>
    //    public IEnumerable<Glyph> Simulate(StateMachine machine, GlyphMetadata metadata, IEnumerable<short> inputGlyphIds)
    //    {
    //        return this.Simulate(machine, metadata, inputGlyphIds.Select(glyphId => new Glyph { GlyphId = glyphId }));
    //    }

    //    /// <summary>
    //    /// Simulates the specified machine on a specified tape.
    //    /// </summary>
    //    /// <param name="machine">The machine.</param>
    //    /// <param name="metadata">The glyph metadata.</param>
    //    /// <param name="inputGlyphs">The input tape</param>
    //    /// <returns>
    //    /// The resulting tape.
    //    /// </returns>
    //    public IEnumerable<Glyph> Simulate(StateMachine machine, GlyphMetadata metadata, IEnumerable<Glyph> inputGlyphs)
    //    {
    //        var tape = new LinkedList<Glyph>(inputGlyphs);

    //        var currentNode = tape.First;
    //        var currentState = machine.EntryState;

    //        // Unlike actual entry state, the implicit fallback state must not advance the tape. However, after a single visit of this state, the tape must start advancing,
    //        // because otherwise the machine would get stuck, if it could not recognize a glyph after going to the implicit fallback state.
    //        var implicitFallbackState = new SubstitutionAction 
    //        { 
    //            Transitions = machine.EntryState.Transitions.Append(new FallbackTransition(machine.EntryState)).ToList()
    //        };
    //        var implicitTransition = new FallbackTransition(implicitFallbackState);

    //        // This stuff is just for debuging purposes
    //        /*
    //        var objectIdToStateNumber = new Dictionary<int, int>();
    //        Console.WriteLine("TRANSITIONS");
    //        int i = 1;
    //        foreach (var state in machine.Transitions)
    //        {
    //            objectIdToStateNumber[RuntimeHelpers.GetHashCode(state)] = i;
    //            i++;
    //        }

    //        Console.WriteLine("STATES");
            
    //        i = 1;
    //        foreach (var state in machine.States)
    //        {
    //            Console.WriteLine(i + " " + state + " {" + string.Join(" ", state.Transitions.Select(p => objectIdToStateNumber[RuntimeHelpers.GetHashCode(p)])) + "} " + RuntimeHelpers.GetHashCode(state));
    //            objectIdToStateNumber[RuntimeHelpers.GetHashCode(state)] = i;
    //            i++;
    //        }

    //        // This stuff is just for debuging purposes
    //        Console.WriteLine("TRANSITIONS");
    //        i = 1;
    //        foreach (var state in machine.Transitions)
    //        {
    //            Console.WriteLine(i + " " + state + " => " + objectIdToStateNumber[RuntimeHelpers.GetHashCode(state.TargetState)] + "  " + RuntimeHelpers.GetHashCode(state));
    //            objectIdToStateNumber[RuntimeHelpers.GetHashCode(state)] = i;
    //            i++;
    //        }

    //        Console.WriteLine(i + " " + implicitFallbackState + " " + RuntimeHelpers.GetHashCode(implicitFallbackState) + " Implicit fallback");
    //        objectIdToStateNumber[RuntimeHelpers.GetHashCode(implicitFallbackState)] = i;*/

    //        // TODO: Ignorovat head shift, pokud projde pres neimplicitni fallback hranu.
    //        int lastHeadShift = 0;
    //        while (currentNode != null)
    //        {
    //            var currentGlyphId = currentNode.Value.GlyphId;

    //            /*Console.WriteLine("===");
    //            Console.WriteLine("Symbol: " + currentGlyphId);
    //            Console.WriteLine("State:" + currentState + " " + (objectIdToStateNumber.ContainsKey(RuntimeHelpers.GetHashCode(currentState)) ? objectIdToStateNumber[RuntimeHelpers.GetHashCode(currentState)] : -1));
    //            */
    //            // Find transition to traverse                
    //            var foundTransition = this.FindTransition(currentState, currentGlyphId, implicitTransition);
    //            /*if (foundTransition.IsFallback && foundTransition != implicitTransition)
    //            {
                    
    //            }*/
    //            /*
    //            Console.WriteLine("Transition: " + foundTransition);

    //            Console.WriteLine("Transition target:" + foundTransition.TargetState + " " + (objectIdToStateNumber.ContainsKey(RuntimeHelpers.GetHashCode(foundTransition.TargetState)) ? objectIdToStateNumber[RuntimeHelpers.GetHashCode(foundTransition.TargetState)] : -1));

    //            Console.WriteLine("Tape (before substitution): " + string.Join(" ", tape.Select(p => p.ToString()))); 
    //            */
    //            // Traverse the transition
    //            currentState = foundTransition.TargetState;

    //            // Advance the tape
    //            if (foundTransition is FallbackTransition && (foundTransition as FallbackTransition).UndoPreviousTapeShift && foundTransition != implicitTransition)
    //            {
    //                Console.WriteLine("Unshifting " + lastHeadShift);
    //                currentNode = currentNode.PreviousBy(lastHeadShift);
    //            }

    //            // Execute replacement/positioning
    //            if (currentState is SubstitutionAction)
    //            {
    //                this.DoSubstitution(tape, metadata, ref currentNode, currentState as SubstitutionAction);
    //            }
    //            else
    //            {
    //                throw new InvalidOperationException("Unknown state type.");
    //            }

    //            Console.WriteLine("Tape (after substitution): " + string.Join(" ", tape.Select(p => p.ToString()))); 

    //            bool shiftForward = currentState.HeadShift > 0;
    //            int shiftedBy = 0;
    //            while (shiftedBy != Math.Abs(currentState.HeadShift) && currentNode != null)
    //            {
    //                currentNode = shiftForward ? currentNode.Next : currentNode.Previous;

    //                if (currentNode != null && !metadata.IsGlyphIgnored(currentNode.Value.GlyphId, currentState.LookupFlags))
    //                {
    //                    shiftedBy++;
    //                }
    //            }

    //            Console.WriteLine("Shift: " + currentState.HeadShift + " to " + (currentNode != null ? currentNode.Value.GlyphId.ToString(CultureInfo.InvariantCulture) : "NULL"));

    //            //else
    //            //{
    //                //currentNode = currentNode.NextBy(currentState.HeadShift);
    //                lastHeadShift = currentState.HeadShift;
    //            //}

    //        }

    //        return tape.ToList();
    //    }

    //    /// <summary>
    //    /// Performs substitution on given cell in the tape.
    //    /// </summary>
    //    /// <param name="tape">The tape.</param>
    //    /// <param name="metadata">The glyph metadata.</param>
    //    /// <param name="currentNode">The current node. This node may get modified.</param>
    //    /// <param name="SubstitutionAction">The substitution state.</param>
    //    /// <exception cref="System.InvalidOperationException">Null currentNode.</exception>
    //    /// <remarks>
    //    /// The current node will by modified according to these rules (with descending priority):
    //    /// 1) If there are any glyphs being added to the tape (this includes glyphs being replaced), the head will point to the first newly added glyph.
    //    /// 2) If glyphs were removed and included the first glyph on the tape, the head will point to the new first glyph.
    //    /// 3) If glyphs were removed, the head will point to the first glyph before those being removed.
    //    /// 4) Otherwise the head will point to the same glyph on which is pointed when this method was called.
    //    /// </remarks>
    //    public void DoSubstitution(LinkedList<Glyph> tape, GlyphMetadata metadata, ref LinkedListNode<Glyph> currentNode, SubstitutionAction SubstitutionAction)
    //    {
    //        if (currentNode == null)
    //        {
    //            throw new ArgumentNullException("currentNode");
    //        }

    //        var startingNode = currentNode;
    //        var insertNode = currentNode;
    //        var removedGlyphs = 0;
    //        while (removedGlyphs != SubstitutionAction.ReplacedGlyphCount)
    //        {
    //            if (currentNode == null)
    //            {
    //                // Unreachable.
    //                throw new InvalidOperationException("Null currentNode.");
    //            }

    //            var previousNode = currentNode.Previous;

    //            if (!metadata.IsGlyphIgnored(currentNode.Value.GlyphId, SubstitutionAction.LookupFlags))
    //            {
    //                removedGlyphs++;
    //                tape.Remove(currentNode);
    //            }
    //            else if (insertNode == startingNode)
    //            {
    //                // If any node is skipped, the new nodes will be inserted AFTER the last (in order of the tape) skipped tode.
    //                insertNode = currentNode;
    //            }
                              
    //            currentNode = previousNode;
    //        }

    //        if (insertNode == startingNode)
    //        {
    //            insertNode = currentNode;
    //        }

    //        if (currentNode == null)
    //        {
    //            currentNode = tape.First;
    //        }

    //        foreach (var glyphId in SubstitutionAction.ReplacementGlyphIds.Reverse())
    //        {
    //            currentNode = tape.AddAfter(insertNode, new Glyph { GlyphId = glyphId });                
    //        }
    //    }

    //    /// <summary>
    //    /// Determines which transition to use if the machine is in given state, the head is pointing to specific symbol and specific features are enabled.
    //    /// </summary>
    //    /// <param name="currentState">The current state.</param>
    //    /// <param name="currentGlyphId">The current glyph id.</param>
    //    /// <param name="implicitTransition">The implicit transition to use, when no other transition is found.</param>
    //    /// <returns>The found transition.</returns>
    //    private ITransition FindTransition(IState currentState, short currentGlyphId, ITransition implicitTransition)
    //    {
    //        return

    //            // Normal transitions
    //            currentState.GetTransitionByGlyphId(currentGlyphId, false) ??

    //            // Fallback transitions
    //            currentState.GetTransitionByGlyphId(currentGlyphId, true) ??

    //            // Implicit transition
    //            implicitTransition;
    //    }
    //}
}
