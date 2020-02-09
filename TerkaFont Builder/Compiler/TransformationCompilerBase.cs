namespace Terka.FontBuilder.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Parser.Output;
    using Terka.FontBuilder.Parser.Output.Context;

    public enum ContextZone
    {
        Context,
        Lookback,
        Lookahead
    }

    /// <summary>
    /// Base class for substitution and positioning compilers.
    /// </summary>
    public abstract class TransformationCompilerBase
    {
        /// <summary>
        /// Creates a transition to be used in context transformation transition paths.
        /// </summary>
        /// <typeparam name="TContextItem">The type of the context item.</typeparam>
        /// <param name="contextItem">The context item.</param>
        /// <param name="headShift">The head shift.</param>
        /// <param name="lookupFlags">The lookup flags.</param>
        /// <param name="transitionAction">The transition action.</param>
        /// <param name="contextZone">The context zone of the context item.</param>
        /// <returns>Transition created based on the parameters.</returns>
        public delegate ITransition CreateContextTransformationTransitionDelegate<in TContextItem>(
            TContextItem contextItem, 
            int headShift, 
            LookupFlags lookupFlags, 
            ITransitionAction transitionAction, 
            ContextZone contextZone);

        /// <summary>
        /// Compiles a collection of transformation tables into a state machine.
        /// </summary>
        /// <param name="transformations">The transformations.</param>
        /// <returns>
        /// Compiled state machine.
        /// </returns>
        public StateMachine Compile(IEnumerable<IGlyphTransformationTable> transformations)
        {
            var builder = new StateMachineBuilder();

            foreach (var transformation in transformations)
            {
                this.CompileTransformation(transformation, builder);
            }

            return builder.GetStateMachine();
        }

        /// <summary>
        /// Compiles a single transformation.
        /// </summary>
        /// <param name="transformation">The transformation.</param>
        /// <param name="builder">The builder.</param>
        public virtual void CompileTransformation(IGlyphTransformationTable transformation, IStateMachineBuilder builder)
        {
            if (transformation is GlyphContextTransformationTable)
            {
                this.CompileRuleContextTransformation(
                    transformation, 
                    builder, 
                    glyphId => glyphId,
                    (glyphId, headShift, lookupFlags, action, zone) => new SimpleTransition {
                        GlyphId = glyphId, 
                        HeadShift = headShift,
                        LookupFlags = lookupFlags,
                        Action = action
                    });
            }
            else if (transformation is ClassContextTransformationTable)
            {
                var table = transformation as ClassContextTransformationTable;
                var classAssignments = table.ClassDefinitions.ClassAssignments;

                /* Here is slight difference from spec - first transition of each "rule" state chain used to recognize the context
                 * will alway be a set transition where glyph set will be defined by appropriate class definition. Instead, only glyps
                 * which are mentioned in the coverage table should be recognized by the first transition. */
                this.CompileRuleContextTransformation(
                    transformation, 
                    builder, 
                    glyphId => table.ClassDefinitions.ClassAssignments.Single(classAssignment => classAssignment.Contains(glyphId)).Key,
                    (classId, headShift, lookupFlags, action, zone) => new SetTransition {
                        GlyphIdSet = new HashSet<ushort>(classAssignments[classId]),
                        HeadShift = headShift,
                        LookupFlags = lookupFlags,
                        Action = action
                    });
            }
            else if (transformation is CoverageContextTransformationTable)
            {
                var table = transformation as CoverageContextTransformationTable;

                foreach (var transformationSet in table.TransformationSets)
                {
                    this.CompileSingleContextTransformation(
                        table, 
                        builder, 
                        transformationSet,
                        (coverageTable, headShift, lookupFlags, action, zone) => new SetTransition{
                            GlyphIdSet = new HashSet<ushort>(coverageTable.CoveredGlyphIds.Values), 
                            HeadShift = headShift,
                            LookupFlags = lookupFlags,
                            Action = action
                        },
                        table.Coverages);
                }
            }
            else if (transformation is ChainingGlyphContextTransformationTable)
            {
                this.CompileChainingRuleContextTransformation(
                    transformation,
                    builder,
                    glyphId => glyphId,
                    (glyphId, headShift, lookupFlags, action, zone) => new SimpleTransition
                    {
                        GlyphId = glyphId,
                        HeadShift = headShift,
                        LookupFlags = lookupFlags,
                        Action = action
                    });
            }
            else if (transformation is ChainingClassContextTransformationTable)
            {
                var table = transformation as ChainingClassContextTransformationTable;
                var contextClassAssignments = table.ContextClassDefinitions.ClassAssignments;
                var lookbackClassAssignments = table.LookbackClassDefinition.ClassAssignments;
                var lookaheadClassAssignments = table.LookaheadClassDefinition.ClassAssignments;

                var zoneToClassesMap = new Dictionary<ContextZone, ILookup<ushort, ushort>>
                {
                    { ContextZone.Context, contextClassAssignments },
                    { ContextZone.Lookback, lookbackClassAssignments },
                    { ContextZone.Lookahead, lookaheadClassAssignments }
                };

                this.CompileChainingRuleContextTransformation(
                    transformation,
                    builder,
                    glyphId => contextClassAssignments.Single(classAssignment => classAssignment.Contains(glyphId)).Key,
                    (classId, headShift, lookupFlags, action, zone) => new SetTransition {
                        GlyphIdSet = new HashSet<ushort>((IEnumerable<ushort>)zoneToClassesMap[zone].SingleOrDefault(p => p.Key == classId) ?? new List<ushort>()),
                        HeadShift = headShift,
                        LookupFlags = lookupFlags,
                        Action = action
                    });
            }
            else if (transformation is ChainingCoverageContextSubstitutionTable)
            {
                var table = transformation as ChainingCoverageContextSubstitutionTable;

                foreach (var transformationSet in table.TransformationSets)
                {
                    this.CompileSingleContextTransformation(
                        table,
                        builder,
                        transformationSet,
                        (coverageTable, headShift, lookupFlags, action, zone) => new SetTransition {
                            GlyphIdSet = new HashSet<ushort>(coverageTable.CoveredGlyphIds.Values), 
                            HeadShift = headShift,
                            LookupFlags = lookupFlags,
                            Action = action
                        },
                        table.ContextCoverages,
                        table.LookbackCoverages,
                        table.LookaheadCoverages);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("transformation");
            }
        }

        /// <summary>
        /// Compiles a rule context transformation.
        /// </summary>
        /// <param name="transformation">The transformation.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="glyphIdToRuleIndexCallback">The callback used to resolve rule index from glyph IDs.</param>
        /// <param name="createTransitionCallback">The callback used to create transitions.</param>
        /// <exception cref="System.InvalidOperationException">Transformations in transformation set don't share LookupFlags.</exception>
        private void CompileRuleContextTransformation(IGlyphTransformationTable transformation, IStateMachineBuilder builder, Func<ushort, ushort> glyphIdToRuleIndexCallback, CreateContextTransformationTransitionDelegate<ushort> createTransitionCallback)
        {
            var table = (ContextRuleTransformationTableBase)transformation;

            // Flatten the collection of rules and associate each rule with glyph received from the coverage table
            var rulesWithFirstGlyph = table
                .TransformationRules
                .Zip(table.Coverage.CoveredGlyphIds.Values)
                .SelectMany(ruleSetWithFirstGlyph => ruleSetWithFirstGlyph.Item1.Select(rule => Tuple.Create(rule, glyphIdToRuleIndexCallback(ruleSetWithFirstGlyph.Item2))));

            foreach (var ruleTuple in rulesWithFirstGlyph)
            {
                foreach (var transformationSet in ruleTuple.Item1.TransformationSets)
                {
                    var completeContext = ruleTuple.Item1.Context.Prepend(ruleTuple.Item2).ToList();

                    this.CompileSingleContextTransformation(table, builder, transformationSet, createTransitionCallback, completeContext);
                }
            }
        }

        /// <summary>
        /// Compiles a rule context transformation.
        /// </summary>
        /// <param name="transformation">The transformation.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="glyphIdToRuleIndexCallback">The callback used to resolve rule index from glyph IDs.</param>
        /// <param name="createTransitionCallback">The callback used to create transitions.</param>
        /// <exception cref="System.InvalidOperationException">Transformations in transformation set don't share LookupFlags.</exception>
        private void CompileChainingRuleContextTransformation(
            IGlyphTransformationTable transformation, 
            IStateMachineBuilder builder, Func<ushort, ushort> glyphIdToRuleIndexCallback,
            CreateContextTransformationTransitionDelegate<ushort> createTransitionCallback)
        {
            var table = (ChainingRuleContextTransformationTableBase)transformation;

            // Flatten the collection of rules and associate each rule with glyph received from the coverage table
            var rulesWithFirstGlyph = table
                .TransformationRules
                .Zip(table.Coverage.CoveredGlyphIds.Values)
                .SelectMany(ruleSetWithFirstGlyph => ruleSetWithFirstGlyph.Item1.Select(rule => Tuple.Create(rule, glyphIdToRuleIndexCallback(ruleSetWithFirstGlyph.Item2))));

            foreach (var ruleTuple in rulesWithFirstGlyph)
            {
                foreach (var transformationSet in ruleTuple.Item1.TransformationSets)
                {
                    var completeInputContext = 
                        ruleTuple.Item1.Context
                        .Prepend(ruleTuple.Item2)
                        .ToList();

                    this.CompileSingleContextTransformation(
                        table, 
                        builder, 
                        transformationSet, 
                        createTransitionCallback, 
                        completeInputContext, 
                        ruleTuple.Item1.Lookback, 
                        ruleTuple.Item1.Lookahead);
                }
            }
        }

        /// <summary>
        /// Compiles a context transformation for a single context.
        /// </summary>
        /// <typeparam name="TContextItem">The type of items in the context. This is arbitrary - it is only used to construct individual transitions in the context recognition section of the machine.</typeparam>
        /// <param name="transformation">The transformation.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="subTransformationSet">The set of tranformation to execute on the matched context.</param>
        /// <param name="createTransitionCallback">The callback used to create transitions from individual items of the context. </param>
        /// <param name="context">The context.</param>
        /// <param name="lookbackContext">The lookback context (NULL if the transition is not chaining).</param>
        /// <param name="lookaheadContext">The lookahead context (NULL if the transition is not chaining). If lookaheadContext is defined, lookbackContext must be defined as well.</param>
        /// <exception cref="System.InvalidOperationException">Transformations in transformation set don't share LookupFlags.</exception>
        private void CompileSingleContextTransformation<TContextItem>(
            IGlyphTransformationTable transformation, 
            IStateMachineBuilder builder, 
            ContextTransformationSet subTransformationSet,
            CreateContextTransformationTransitionDelegate<TContextItem> createTransitionCallback, 
            IEnumerable<TContextItem> context, 
            IEnumerable<TContextItem> lookbackContext = null, 
            IEnumerable<TContextItem> lookaheadContext = null)
        {
            if (!subTransformationSet.Transformations.Any())
            {
                return;
            }

            var contextList = context.ToList();
            var lookbackContextList = lookbackContext == null ? new List<TContextItem>() : lookbackContext.Reverse().ToList();
            var lookaheadContextList = lookaheadContext == null ? new List<TContextItem>() : lookaheadContext.ToList();

            var completeContext = lookbackContextList.Append(contextList).Append(lookaheadContextList).ToList();

            var subMachineLookupFlags = subTransformationSet.Transformations.First().LookupFlags;

            // All the transformations in one transformation set must have the same lookup flags (they are supposed to come from
            // a single lookup).
            if (subTransformationSet.Transformations.Any(p => p.LookupFlags != subMachineLookupFlags))
            {
                throw new InvalidOperationException("Transformations in transformation set don't share LookupFlags.");
            }

            // The head must end pointing to the first glyph after end of the context
            var subBuilder = new SubMachineBuilder(subMachineLookupFlags);

            foreach (var subTransformation in subTransformationSet.Transformations)
            {
                this.CompileTransformation(subTransformation, subBuilder);
            }

            /* The path for each rule is constructed as follows:
                 *  1) A chain of states which match the context (including lookback and lookahead context).
                 *  2) State which positions the head to adhere to first glyph index of the transformation set
                 *      and creates the context terminator glyph. The transition checking the last glyph of the context 
                 *      leads to this state. Additonal state is inserted before this one in case the table has defined a 
                 *      lookahead
                 *  3) The sub-machine to be applied on the context. This state machine is modified to jump to 4 whenever
                 *      it encounters context terminator. 
                 *  4) State which positions the head to end of the context and removes the context terminator. */
            
            // Part 1. Doesn't include the transition and state checking the last glyph (see part 2).
            var contextMatchPathSegment = completeContext
                .Take(completeContext.Count - 1)
                .Select(
                    (contextMember, index) =>
                        createTransitionCallback(
                            contextMember,
                            1,
                            transformation.LookupFlags,
                            null,
                            this.GetContextZoneForGlyphIndex(index, contextList, lookbackContextList, lookaheadContextList))).ToList();

            List<ITransition> subMachineInitializationPathSegment;
            if (lookaheadContextList.Count > 0)
            {
                // Part 2. Sub-machine initialization state.
                subMachineInitializationPathSegment = new List<ITransition>
                {
                    createTransitionCallback(
                        completeContext.Last(),
                        -lookaheadContextList.Count,
                        transformation.LookupFlags,
                        null,
                        this.GetContextZoneForGlyphIndex(completeContext.Count - 1, contextList, lookbackContextList, lookaheadContextList)),
                    new AlwaysTransition
                    {       
                        HeadShift = -contextList.Count,
                        LookupFlags = transformation.LookupFlags,
                        Action = new SubstitutionAction
                        {
                            ReplacedGlyphCount = 0,
                            ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId }
                        }
                    }
                };  
            }
            else
            {
                // Part 2. Sub-machine initialization state.
                subMachineInitializationPathSegment = new List<ITransition>
                {
                    createTransitionCallback(
                        completeContext.Last(),
                        -contextList.Count,
                        transformation.LookupFlags,
                        new SubstitutionAction
                        {
                            ReplacementGlyphIds = new[] { SubMachineBuilder.ContextTerminatorGlyphId }
                        },
                        this.GetContextZoneForGlyphIndex(completeContext.Count - 1, contextList, lookbackContextList, lookaheadContextList))
                };                
            }


            /* Part 3 a 4. Sub-machine iself + deinitialization state. 
                 * Including its entry state (to which lead all the internal transitions). */
            var subMachinePaths = subBuilder.GetPaths();

            // Assemble the paths and add them into the outer machine builder.
            foreach (var subMachinePath in subMachinePaths)
            {
                var assembledPath = contextMatchPathSegment.Append(subMachineInitializationPathSegment).Append(subMachinePath).ToList();

                builder.AddPath(assembledPath);
            }
        }

        /// <summary>
        /// Identifies a context zone in which a context item is by its index.
        /// </summary>
        /// <typeparam name="TContextItem">The type of the context item.</typeparam>
        /// <param name="index">The index.</param>
        /// <param name="context">The context.</param>
        /// <param name="lookback">The lookback.</param>
        /// <param name="lookahead">The lookahead.</param>
        /// <returns>The context zone of the context item.</returns>
        private ContextZone GetContextZoneForGlyphIndex<TContextItem>(int index, List<TContextItem> context, List<TContextItem> lookback, List<TContextItem> lookahead)
        {
            if (index < lookback.Count)
            {
                return ContextZone.Lookback;
            }
            
            if (index < lookback.Count + context.Count)
            {
                return ContextZone.Context;
            }
            
            return ContextZone.Lookahead;
        }
    }
}