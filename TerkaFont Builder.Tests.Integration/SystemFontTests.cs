namespace Terka.TerkaBuilder.IntegrationTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using NUnit.Framework;

    using Terka.FontBuilder;
    using Terka.FontBuilder.Compiler;
    using Terka.FontBuilder.Extensions;
    using Terka.FontBuilder.Optimizer;
    using Terka.FontBuilder.Parser;
    using Terka.FontBuilder.Parser.Output.Positioning;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
    // ReSharper disable UnusedVariable

    /// <summary>
    /// Contains tests which try parser and compiler at system fonts (mostly to see that they don't crash).
    /// </summary>
    [TestFixture]
    public class SystemFontTests
    {
        [Test]
        public void FullProcessOnCalibriiSerbian()
        {
            var allFontPaths = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "*.ttf");
            var typeface = new GlyphTypeface(new Uri(allFontPaths.Select(p => p.ToLower()).First(p => p.Contains("calibrii"))));

            var gsubParser = new GsubParser();
            var gposParser = new GposParser();

            var script = new Tag("cyrl");
            var langSys = new Tag("SRB ");

            // Parse the table information from the font
            var substitutionFeatures = gsubParser.GetOptionalFeatureTags(typeface, script, langSys).ToList();
            var positioningFeatures = gposParser.GetOptionalFeatureTags(typeface, script, langSys).ToList();

            var requiredSubstitutions = gsubParser.GetTransformationTablesForRequiredFeature(typeface, script, langSys);
            var optionalSubstitutionsByFeature =
              (from chosenOptionalFeatureTag in substitutionFeatures
               from substitutionTable in gsubParser.GetTransformationTablesForOptionalFeature(typeface, script, langSys, chosenOptionalFeatureTag)
               select new { Key = chosenOptionalFeatureTag, Value = substitutionTable }).ToLookup(p => p.Key, p => p.Value);
            var allSubstitutions = new [] { requiredSubstitutions }.Append(optionalSubstitutionsByFeature).ToList();

            var requiredPositioning = gposParser.GetTransformationTablesForRequiredFeature(typeface, script, langSys);
            var optionalPositioningByFeature =
              (from chosenOptionalFeatureTag in positioningFeatures
               from positioningTable in gposParser.GetTransformationTablesForOptionalFeature(typeface, script, langSys, chosenOptionalFeatureTag)
               select new { Key = chosenOptionalFeatureTag, Value = positioningTable }).ToLookup(p => p.Key, p => p.Value);
            var allPositioning = new [] { requiredPositioning }.Append(optionalPositioningByFeature).ToList();

            // Compile the parsed tables into state machines
            var gsubCompiler = new SubstitutionCompiler();
            var gposCompiler = new PositioningCompiler();

            var SubstitutionActionMachines = allSubstitutions.Select(gsubCompiler.Compile);
            var positioningStateMachines = allPositioning.Select(gposCompiler.Compile);
            var allStateMachines = SubstitutionActionMachines.Append(positioningStateMachines).ToList();

            // Optimize the state machines
            var optimizer = new StateMachineOptimizer();
            var optimizedStateMachines = allStateMachines.Select(optimizer.Optimize).ToList();

            // ..and normalize the machines
            var normalizer = new StateMachineNormalizer();
            var finishedStateMachines = optimizedStateMachines.Select(normalizer.Normalize).ToList();

            // Sometimes the tables contain empty features (eg. in case of required features).
            var writableStateMachines = finishedStateMachines.Where(p => p.States.Count > 1).ToList();
        }

        /// <summary>
        /// Tests that GSUB parser and compiler work on all system fonts, in all their script, language systems and features.
        /// </summary>
        [Test]
        public void GsubCompilerAndParserWorkOnAllSystemFonts()
        {
            var allFontPaths = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "*.ttf");

            Parallel.ForEach(allFontPaths, fontPath =>
            {
                Console.WriteLine(fontPath);

                var typeface = new GlyphTypeface(new Uri(fontPath));

                var gsubParser = new GsubParser();
                var gsubCompiler = new SubstitutionCompiler();

                var scripts = gsubParser.GetScriptTags(typeface).ToList();
                foreach (var script in scripts)
                {
                    Console.WriteLine(fontPath + " - " + script.Label);

                    var langSyss = gsubParser.GetLangSysTags(typeface, script).ToList();
                    foreach (var langSys in langSyss)
                    {
                        Console.WriteLine(fontPath + " - " + script.Label + " - " + langSys.Label);

                        var features = gsubParser.GetOptionalFeatureTags(typeface, script, langSys).ToList();

                        // Parse the tables
                        var requiredTransformations = gsubParser.GetTransformationTablesForRequiredFeature(typeface, script, langSys).ToList();
                        gsubCompiler.Compile(requiredTransformations);

                        var optionalTransformationGroups =
                          (from chosenOptionalFeatureTag in features
                           select gsubParser.GetTransformationTablesForOptionalFeature(typeface, script, langSys, chosenOptionalFeatureTag)).ToList();

                        foreach (var optionalTransformationGroup in optionalTransformationGroups)
                        {
                            optionalTransformationGroup.ToList();
                            gsubCompiler.Compile(optionalTransformationGroup);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Tests that GPOS parser and compiler work on all system fonts, in all their script, language systems and features.
        /// </summary>
        [Test]
        public void GposCompilerAndParserWorkOnAllSystemFonts()
        {
            var allFontPaths = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "*.ttf");

            foreach (var fontPath in allFontPaths)
            {
                Console.WriteLine(fontPath);

                var typeface = new GlyphTypeface(new Uri(fontPath));

                var gposParser = new GposParser();
                var gposCompiler = new PositioningCompiler();

                var scripts = gposParser.GetScriptTags(typeface).ToList();
                foreach (var script in scripts)
                {
                    Console.WriteLine(fontPath + " - " + script.Label);

                    var langSyss = gposParser.GetLangSysTags(typeface, script).ToList();
                    foreach (var langSys in langSyss)
                    {
                        Console.WriteLine(fontPath + " - " + script.Label + " - " + langSys.Label);

                        var features = gposParser.GetOptionalFeatureTags(typeface, script, langSys).ToList();

                        // Parse the tables
                        var requiredTransformations = gposParser.GetTransformationTablesForRequiredFeature(typeface, script, langSys).ToList();
                        gposCompiler.Compile(requiredTransformations);

                        var optionalTransformationGroups =
                            (from chosenOptionalFeatureTag in features
                             select gposParser.GetTransformationTablesForOptionalFeature(typeface, script, langSys, chosenOptionalFeatureTag)).ToList();

                        foreach (var optionalTransformationGroup in optionalTransformationGroups)
                        {
                            optionalTransformationGroup.ToList();
                            gposCompiler.Compile(optionalTransformationGroup);
                        }
                    }

                    Console.WriteLine("/ " + fontPath + " - " + script.Label);
                }
            };
        }
    }
}
