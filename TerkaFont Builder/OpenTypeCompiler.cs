using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Terka.FontBuilder.Compiler;
using Terka.FontBuilder.Compiler.Output;
using Terka.FontBuilder.Optimizer;
using Terka.FontBuilder.Parser;
using Terka.FontBuilder.Parser.Reflection;
using Terka.TinyFonts;

namespace Terka.FontBuilder
{
    /// <summary>
    /// OpenType features Compiler.
    /// </summary>
    public class OpenTypeCompiler : IOpenTypeCompiler
    {
        private static Dictionary<string, StateMachine> _machineCache;

        private GsubParser _gsubParser = new GsubParser();
        private GposParser _gposParser = new GposParser();

        static OpenTypeCompiler()
        {
            _machineCache = new Dictionary<string, StateMachine>();
        }

        private static string ToTagString(uint tag)
        {
            return new string(new[] 
            {
                (char)((tag & 0xFF000000) >> 24),
                (char)((tag & 0x00FF0000) >> 16),
                (char)((tag & 0x0000FF00) >> 8) ,
                (char)((tag & 0x000000FF))}
            );
        }

        /// <summary>
        /// Checks if substitution feature is present in glyph <paramref name="typeface"/>.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>True if feature is present.</returns>
        public virtual bool IsSubstitutionFeaturePresent(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId)
        {
            return IsFeaturePresent(_gsubParser, typeface, scriptId, languageId, featureId);
        }
        /// <summary>
        /// Checks if positioning feature is present in glyph <paramref name="typeface"/>.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look feature up.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>True if feature is present.</returns>
        public virtual bool IsPositioningFeaturePresent(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId)
        {
            return IsFeaturePresent(_gposParser, typeface, scriptId, languageId, featureId);
        }
        private bool IsFeaturePresent(TransformationParserBase parser, GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId)
        {
            Tag scriptTest = TagConverter.TagFromUint(scriptId);
            Tag languageTest = TagConverter.TagFromUint(languageId);
            Tag featureTest = TagConverter.TagFromUint(featureId);

            foreach (Tag script in parser.GetScriptTags(typeface))
            {
                if (script == scriptTest)
                {
                    foreach (Tag language in parser.GetLangSysTags(typeface, script))
                    {
                        if (language == languageTest)
                        {
                            foreach (Tag feature in parser.GetAllFeatureTags(typeface))
                            {
                                if (feature == featureTest)
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets generated glyphs during compilation of state machine.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <returns>All used glyphs during compilation.</returns>
        public virtual IEnumerable<ushort> GetGeneratedGlyphIds(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId)
        {
            StateMachine machine = GetOrCompile(_gsubParser, new SubstitutionCompiler(), typeface, scriptId, languageId, featureId);
            return machine.GetGeneratedGlyphIds();
        }

        /// <summary>
        /// Compiles state machine for substitution feature and saves state machine to <paramref name="substitution"/> appendix.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <param name="substitution">Substitution appendix in which will be compiled state machine stored.</param>
        /// <param name="glyphClasses">Glyph classes appendix for use by substitution appendix.</param>
        /// <param name="availableGlyphs">Which glyphs should be restricted in state machine.</param>
        /// <returns>All used glyphs during compilation. Can add additional glyphs to availableGlyphs.</returns>
        public virtual IEnumerable<ushort> CompileFeature(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId, SubstitutionAppendix substitution, GlyphClassesAppendix glyphClasses, IEnumerable<ushort> availableGlyphs)
        {
            StateMachine machine = GetOrCompile(_gsubParser, new SubstitutionCompiler(), typeface, scriptId, languageId, featureId);

            machine = new StateMachineOptimizer().Optimize(machine);
            machine = new StateMachineNormalizer().Normalize(machine, availableGlyphs);

            this.CompileSubstitutionAppendingFromMachine(featureId, substitution, glyphClasses, machine);

            return machine.GetGeneratedGlyphIds();
        }

        protected void CompileSubstitutionAppendingFromMachine(uint featureId, SubstitutionAppendix substitution, GlyphClassesAppendix glyphClasses, StateMachine machine)
        {
            Debug.Assert(machine.States[0] == machine.EntryState, "First state is not the entry state.");

            SubstitutionAppendix.Feature feature = new SubstitutionAppendix.Feature();

            checked
            {
                for (ushort requiredState = 0; requiredState < machine.States.Count; requiredState++)
                {
                    var state = machine.States[requiredState];

                    foreach (var transition in state.Transitions)
                    {
                        SubstitutionAppendix.Rule rule = new SubstitutionAppendix.Rule();
                        rule.RequiredState = requiredState;
                        rule.NewState = (ushort)machine.States.IndexOf(transition.TargetState);

                        if (transition is AlwaysTransition)
                        {
                            rule.Condition = SubstitutionAppendix.RuleCondition.Unconditional;
                        }
                        else if (transition is SimpleTransition)
                        {
                            rule.Condition = SubstitutionAppendix.RuleCondition.Glyph;
                            rule.ConditionParameter = ((SimpleTransition)transition).GlyphId;
                        }
                        else if (transition is SetTransition)
                        {
                            SetTransition setTransition = (SetTransition)transition;
                            int[] glyphs = setTransition.GlyphIdSet.Select(id => (int)id).ToArray();

                            GlyphClassesAppendix.Coverage coverage = glyphClasses.FindCoverage(glyphs);
                            if (coverage == null)
                            {
                                coverage = glyphClasses.AppendCoverage(glyphs);
                            }

                            if (!glyphClasses.Coverages.Contains(coverage))
                            {
                                glyphClasses.Coverages.Add(coverage);
                            }

                            rule.Condition = SubstitutionAppendix.RuleCondition.GlyphClass;
                            rule.ConditionParameter = (ushort)glyphClasses.Coverages.IndexOf(coverage);
                        }
                        else
                        {
                            Debug.Assert(false, "Unknown condition: " + transition.GetType());
                            continue;
                        }

                        if (transition.Action == null)
                        {
                            rule.Action = SubstitutionAppendix.RuleAction.Nothing;
                        }
                        else
                        {
                            SubstitutionAction action = transition.Action as SubstitutionAction;
                            if (action == null)
                            {
                                Debug.Assert(false, "Unknown action: " + transition.Action.GetType());
                                continue;
                            }

                            int replacementGlyphCount = action.ReplacementGlyphIds.Count();
                            if (replacementGlyphCount == 1 && action.ReplacedGlyphCount <= 3)
                            {
                                if (action.ReplacedGlyphCount == 0)
                                {
                                    rule.Action = SubstitutionAppendix.RuleAction.GlyphInsertion;
                                }
                                else if (action.ReplacedGlyphCount == 1)
                                {
                                    rule.Action = SubstitutionAppendix.RuleAction.GlyphOverwrite;
                                }
                                else if (action.ReplacedGlyphCount == 2)
                                {
                                    rule.Action = SubstitutionAppendix.RuleAction.GlyphRewrite_2_1;
                                }
                                else if (action.ReplacedGlyphCount == 3)
                                {
                                    rule.Action = SubstitutionAppendix.RuleAction.GlyphRewrite_3_1;
                                }
                                else
                                {
                                    Debug.Assert(false, "Unknown action: " + action);
                                    continue;
                                }

                                rule.ActionParameter = action.ReplacementGlyphIds.First();
                            }
                            else if (replacementGlyphCount == 0)
                            {
                                rule.Action = SubstitutionAppendix.RuleAction.GlyphDeletion;
                                rule.ActionParameter = (ushort)action.ReplacedGlyphCount;
                            }
                            else
                            {
                                rule.Action = SubstitutionAppendix.RuleAction.GlyphRewrite_N_M;
                                rule.ActionParameter = substitution.AppendParameters(new SubstitutionAppendix.GlyphRewriteParameters((byte)action.ReplacedGlyphCount, action.ReplacementGlyphIds.Select(g => (int)g).ToArray()));
                            }

                            rule.ActionOffset = (sbyte)(1 - action.SkippedGlyphCount - action.ReplacedGlyphCount);
                            rule.TapeMovement = (sbyte)action.SkippedGlyphCount;
                        }

                        rule.TapeMovement += (sbyte)(transition.HeadShift);
                        feature.Rules.Add(rule);
                    }
                }
            }

            substitution.Features.Add(feature);
            substitution.FeatureOffsets.Add(new SubstitutionAppendix.FeatureOffset { Tag = featureId });
        }

        /// <summary>
        /// Compiles state machine for positioning feature and saves state machine to <paramref name="positioning"/> appendix.
        /// </summary>
        /// <param name="typeface">Glyph typeface in which look for feature.</param>
        /// <param name="scriptId">ID of script in which look for feature.</param>
        /// <param name="languageId">ID of language in which look for feature.</param>
        /// <param name="featureId">ID of feature to look up.</param>
        /// <param name="positioning">Positioning appendix in which will be compiled state machine stored.</param>
        /// <param name="glyphClasses">Glyph classes appendix for use by positioning appendix.</param>
        /// <param name="availableGlyphs">Which glyphs should be restricted in state machine.</param>
        /// <param name="emSize">Requested em size.</param>
        /// <returns>All used glyphs during compilation. Can add additional glyphs to availableGlyphs.</returns>
        public virtual IEnumerable<ushort> CompileFeature(GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId, PositioningAppendix positioning, GlyphClassesAppendix glyphClasses, IEnumerable<ushort> availableGlyphs, double emSize)
        {
            StateMachine machine = GetOrCompile(_gposParser, new PositioningCompiler(), typeface, scriptId, languageId, featureId);

            machine = new StateMachineOptimizer().Optimize(machine);
            machine = new StateMachineNormalizer().Normalize(machine, availableGlyphs);

            this.CompilePositioningAppendixFromMachine(typeface, featureId, positioning, glyphClasses, emSize, machine);

            return machine.GetGeneratedGlyphIds();
        }

        protected void CompilePositioningAppendixFromMachine(GlyphTypeface typeface, uint featureId, PositioningAppendix positioning, GlyphClassesAppendix glyphClasses, double emSize, StateMachine machine)
        {
            Debug.Assert(machine.States[0] == machine.EntryState, "First state is not the entry state.");

            PositioningAppendix.Feature feature = new PositioningAppendix.Feature();

            checked
            {
                for (ushort requiredState = 0; requiredState < machine.States.Count; requiredState++)
                {
                    var state = machine.States[requiredState];

                    foreach (var transition in state.Transitions)
                    {
                        PositioningAppendix.Rule rule = new PositioningAppendix.Rule();
                        rule.RequiredState = requiredState;
                        rule.NewState = (ushort)machine.States.IndexOf(transition.TargetState);

                        if (transition is AlwaysTransition)
                        {
                            rule.Condition = PositioningAppendix.RuleCondition.Unconditional;
                        }
                        else if (transition is SimpleTransition)
                        {
                            rule.Condition = PositioningAppendix.RuleCondition.Glyph;
                            rule.ConditionParameter = ((SimpleTransition)transition).GlyphId;
                        }
                        else if (transition is SetTransition)
                        {
                            SetTransition setTransition = (SetTransition)transition;
                            int[] glyphs = setTransition.GlyphIdSet.Select(id => (int)id).ToArray();

                            GlyphClassesAppendix.Coverage coverage = glyphClasses.FindCoverage(glyphs);
                            if (coverage == null)
                            {
                                coverage = glyphClasses.AppendCoverage(glyphs);
                            }

                            if (!glyphClasses.Coverages.Contains(coverage))
                            {
                                glyphClasses.Coverages.Add(coverage);
                            }

                            rule.Condition = SubstitutionAppendix.RuleCondition.GlyphClass;
                            rule.ConditionParameter = (ushort)glyphClasses.Coverages.IndexOf(coverage);
                        }
                        else
                        {
                            Debug.Assert(false, "Unknown condition: " + transition.GetType());
                            continue;
                        }

                        if (transition.Action == null)
                        {
                            rule.Action = PositioningAppendix.RuleAction.Nothing;
                        }
                        else
                        {
                            if (transition.Action is AnchorPointToAnchorPointAction)
                            {
                                AnchorPointToAnchorPointAction anchorAction = transition.Action as AnchorPointToAnchorPointAction;

                                sbyte x = ToPixels(anchorAction.PreviousGlyphAnchorPoint.X - anchorAction.CurrentGlyphAnchorPoint.X, emSize, typeface);
                                sbyte y = ToPixels(anchorAction.PreviousGlyphAnchorPoint.Y - anchorAction.CurrentGlyphAnchorPoint.Y, emSize, typeface);

                                rule.Action = PositioningAppendix.RuleAction.PositionOffset;
                                rule.ActionParameter = Pack(x, y);
                                rule.ActionOffset = 0;
                            }
                            else if (transition.Action is PositioningAdjustmentAction)
                            {
                                PositioningAdjustmentAction positioningAction = transition.Action as PositioningAdjustmentAction;
                                List<GlyphPositionChange> changes = positioningAction.PositionChanges.ToList();

                                rule.ActionOffset = (sbyte)(1 - changes.Count);

                                rule.TapeMovement = (sbyte)TrimEnd(changes);
                                if (changes.Count == 0)
                                {
                                    rule.Action = SubstitutionAppendix.RuleAction.Nothing;
                                    rule.ActionOffset = 0;
                                }
                                else
                                {
                                    if (changes.Count == 1)
                                    {
                                        GlyphPositionChange position = changes[0];

                                        if ((position.AdvanceX != 0 || position.AdvanceY != 0) && (position.OffsetX == 0 && position.OffsetY == 0))
                                        {
                                            sbyte x = ToPixels(position.AdvanceX, emSize, typeface);
                                            sbyte y = ToPixels(position.AdvanceY, emSize, typeface);

                                            rule.Action = PositioningAppendix.RuleAction.PositionAdvance;
                                            rule.ActionParameter = Pack(x, y);
                                        }
                                        else if ((position.OffsetX != 0 || position.OffsetY != 0) && (position.AdvanceX == 0 && position.AdvanceY == 0))
                                        {
                                            sbyte x = ToPixels(position.OffsetX, emSize, typeface);
                                            sbyte y = ToPixels(position.OffsetY, emSize, typeface);

                                            rule.Action = PositioningAppendix.RuleAction.PositionOffset;
                                            rule.ActionParameter = Pack(x, y);
                                        }
                                    }

                                    if (rule.Action == StateMachineAppendix.RuleAction.Nothing)
                                    {
                                        PositioningAppendix.PositionChangesParameters parameters = new StateMachineAppendix.PositionChangesParameters();

                                        foreach (GlyphPositionChange position in changes)
                                        {
                                            PositioningAppendix.PositionChange change = new StateMachineAppendix.PositionChange(ToPixels(position.OffsetX, emSize, typeface), ToPixels(position.OffsetY, emSize, typeface), ToPixels(position.AdvanceX, emSize, typeface), ToPixels(position.AdvanceY, emSize, typeface));
                                            parameters.PositionChanges.Add(change);
                                        }

                                        rule.Action = PositioningAppendix.RuleAction.PositionComplex;
                                        rule.ActionParameter = positioning.AppendParameters(parameters);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Assert(false, "Unknown transition action: " + transition.Action.GetType());
                                continue;
                            }
                        }

                        rule.TapeMovement += (sbyte)(transition.HeadShift);
                        feature.Rules.Add(rule);
                    }
                }
            }

            positioning.Features.Add(feature);
            positioning.FeatureOffsets.Add(new SubstitutionAppendix.FeatureOffset { Tag = featureId });
        }

        private static StateMachine GetOrCompile(TransformationParserBase parser, TransformationCompilerBase compiler, GlyphTypeface typeface, uint scriptId, uint languageId, uint featureId)
        {
            Tag scriptTag = new Tag(ToTagString(scriptId));
            Tag languageTag = new Tag(ToTagString(languageId));
            Tag featureTag = new Tag(ToTagString(featureId));

            string key = typeface.FontUri.OriginalString + scriptTag + languageTag + featureTag;

            StateMachine machine;

            if (!_machineCache.TryGetValue(key, out machine))
            {
                IEnumerable<Terka.FontBuilder.Parser.Output.IGlyphTransformationTable> tables;
                if (featureId == 0)
                    tables = parser.GetTransformationTablesForRequiredFeature(typeface, scriptTag, languageTag);
                else
                    tables = parser.GetTransformationTablesForOptionalFeature(typeface, scriptTag, languageTag, featureTag);

                _machineCache[key] = machine = compiler.Compile(tables);
            }

            return machine;
        }

        private static sbyte ToPixels(double designSize, double emSize, GlyphTypeface typeface)
        {
            dynamic typefaceFriend = new AccessPrivateWrapper(typeface);
            ushort designEmHeight = typefaceFriend.DesignEmHeight;

            return (sbyte)(designSize / designEmHeight * (emSize * 96.0 / 72.0));
        }
        private static int TrimEnd(List<GlyphPositionChange> changes)
        {
            int totalCount = changes.Count;

            int i = changes.Count;
            while (--i >= 0 && IsPositionChangeEmpty(changes[i]))
                changes.RemoveAt(i);

            return totalCount - changes.Count;
        }

        private static bool IsPositionChangeEmpty(GlyphPositionChange change)
        {
            return change.AdvanceX == 0 && change.AdvanceY == 0 && change.OffsetX == 0 && change.OffsetY == 0;
        }
        private static ushort Pack(sbyte value1, sbyte value2)
        {
            return unchecked((ushort)((byte)value2 + ((byte)value1 << 8)));
        }
    }
}
