namespace Terka.FontBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using Terka.FontBuilder.Compiler;
    using Terka.FontBuilder.Compiler.Output;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Optimizer;
    using Terka.FontBuilder.Parser;

    /// <summary>
    /// Entry point.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            OpenTypeCompiler openTypecompiler = new OpenTypeCompiler();
            //openTypecompiler.Positioning(, TagConverter.UintFromTag(new Tag("latn")), TagConverter.UintFromTag(new Tag("dflt")), TagConverter.UintFromTag(new Tag("kern")));
            var allFontPaths = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));

            var typeface = new GlyphTypeface(new Uri(allFontPaths.Select(p => p.ToLower()).First(p => p.Contains("pala"))));
            //var typeface = new GlyphTypeface(new Uri(allFontPaths.First(p => p.Contains("arial"))));

            var gdefParser = new GdefParser();
            var ignoreBase = gdefParser.GetGlyphIdsByLookupFlags(typeface, LookupFlags.IgnoreBaseGlyphs);

            var cmapParser = new CmapParser();

            /*Dictionary<short, char> reverse = new Dictionary<short,char>();
            foreach (var pair in cmapParser.GetCharacterToGlyphIdMapping(typeface))
            {
                reverse[pair.Value] = pair.Key;
            }
            var ad = gdefParser.GetGlyphClasses(typeface);
            var ac = gdefParser.GetGlyphClasses(typeface).Where(a => a.Value != 0);
            var ab = gdefParser.GetGlyphClasses(typeface).Where(a => a.Value != 0 && reverse.ContainsKey(a.Key)).ToDictionary(a => reverse[a.Key], a => a.Value);

            var writer = new StreamWriter("glyphs.txt");
            foreach (var glyphClass in ab)
            {
                writer.WriteLine(glyphClass.Key + " => " + glyphClass.Value);
            }
            writer.Close();*/


            var metadata = new GlyphMetadata
            {
                CharacterToGlyphIdMapping = cmapParser.GetCharacterToGlyphIdMapping(typeface).GetValueOrDefault,
                GlyphIdToGlyphClassMapping = ((IDictionary<ushort, GlyphClass>)gdefParser.GetGlyphClasses(typeface)).GetValueOrDefault,
                GlyphIdToMarkAttachClassIdMapping = ((IDictionary<ushort, ushort>)gdefParser.GetMarkAttachClassIds(typeface)).GetValueOrDefault,
            };

            var gsubParser = new GsubParser();

            var scripts = gsubParser.GetScriptTags(typeface).ToList();

            Console.WriteLine("SCRIPTS: ");
            foreach (var tag in scripts)
            {
                Console.WriteLine("\t" + tag.Label);
            }

            var script1 = new Tag("latn");

            var langSyss = gsubParser.GetLangSysTags(typeface, script1).ToList();

            Console.WriteLine("LANGUAGE SYSTEMS: ");
            foreach (var tag in langSyss)
            {
                Console.WriteLine("\t" + tag.Label);
            }

            //var langSys = new Tag("LTH ");
            var langSys = new Tag("dflt");

            var features = gsubParser.GetOptionalFeatureTags(typeface, script1, langSys).ToList();

            Console.WriteLine("OPTIONAL FEATURES: ");
            foreach (var tag in features)
            {
                Console.WriteLine("\t" + tag.Label);
            }

            //gsubParser.Dump(typeface);

            //// var enabledOptionalFeatures = parser.GetOptionalFeatureTags(typeface, script, langSys).Skip(5).Take(5).ToList();
            //var chosenOptionalFeatureTags = new[] { new Tag("liga") };
            var chosenOptionalFeatureTags =  new[] { new Tag("frac") };

            var requiredTransformations = gsubParser.GetTransformationTablesForRequiredFeature(typeface, script1, langSys);
            var optionalTransformationsByFeature =
              (from chosenOptionalFeatureTag in chosenOptionalFeatureTags
               from substitutionTable in gsubParser.GetTransformationTablesForOptionalFeature(typeface, script1, langSys, chosenOptionalFeatureTag)
               select new { Key = chosenOptionalFeatureTag, Value = substitutionTable }).ToLookup(p => p.Key, p => p.Value);

            var compiler = new SubstitutionCompiler();
            var SubstitutionActionMachine = compiler.Compile(requiredTransformations);

            var optionalStateMachinesByFeature = optionalTransformationsByFeature.ToDictionary(p => p.Key, compiler.Compile);

            var machine = optionalStateMachinesByFeature.First().Value;

            var gposParser = new GposParser();
            gposParser.Dump(typeface);
            scripts = gposParser.GetScriptTags(typeface).ToList();

            Console.WriteLine("SCRIPTS: ");
            foreach (var tag in scripts)
            {
                Console.WriteLine("\t" + tag.Label);
                langSyss = gposParser.GetLangSysTags(typeface, script1).ToList();

                foreach (var y in langSyss)
                {
                    Console.WriteLine("\t\t" + y.Label);
                }
            }

            script1 = new Tag("cyrl");

            langSyss = gposParser.GetLangSysTags(typeface, script1).ToList();

            Console.WriteLine("LANGUAGE SYSTEMS: ");
            foreach (var tag in langSyss)
            {
                Console.WriteLine("\t" + tag.Label);
            }

            Console.WriteLine("OPTIONAL FEATURES: ");
            foreach (var tag in features)
            {
                Console.WriteLine("\t" + tag.Label);
            }

            langSys = new Tag("SRB ");

            chosenOptionalFeatureTags = features.ToArray();// new[] { new Tag("kern") };

            gposParser.Dump(typeface);
            var gposCompiler = new PositioningCompiler();

            var positioningFeatures = gposParser.GetOptionalFeatureTags(typeface, script1, langSys).ToList();

            var requiredPositioning = gposParser.GetTransformationTablesForRequiredFeature(typeface, script1, langSys);
            var compiledRequiredPositioning = gposCompiler.Compile(requiredPositioning);


            var optionalPositioningByFeature =
              (from chosenOptionalFeatureTag in positioningFeatures
               from positioningTable in gposParser.GetTransformationTablesForOptionalFeature(typeface, script1, langSys, chosenOptionalFeatureTag)
               select new { Key = chosenOptionalFeatureTag, Value = positioningTable }).ToLookup(p => p.Key, p => p.Value);



            var compiledOptionalPositioning =
                (from positioningTables in optionalPositioningByFeature
                 select new { Value = gposCompiler.Compile(positioningTables), positioningTables.Key }).ToLookup(p => p.Key, p => p.Value);

            var machineToOptimize = compiledOptionalPositioning[new Tag("kern")].Single();

            //var normalizedMachine = normalizer.Normalize(machineToOptimize);

            var optimizer = new StateMachineOptimizer();
            var optimizedMachine = optimizer.Optimize(machineToOptimize);

            var normalizer = new StateMachineNormalizer();
            var optimizedNormalizedMachíne = normalizer.Normalize(optimizedMachine);
        }
    }
}
