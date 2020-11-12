using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using NUnit.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;
#if DIRECTX
using System.Collections.Generic;
using TwoMGFX;
#endif

namespace MonoGame.Tests.ContentPipeline
{
    class EffectProcessorTests
    {
        class ImporterContext : ContentImporterContext
        {
            public override string IntermediateDirectory
            {
                get { throw new NotImplementedException(); }
            }

            public override ContentBuildLogger Logger
            {
                get { throw new NotImplementedException(); }
            }

            public override string OutputDirectory
            {
                get { throw new NotImplementedException(); }
            }

            public override void AddDependency(string filename)
            {
                throw new NotImplementedException();
            }
        }

#if DIRECTX
        [Test]
        public void TestPreprocessor()
        {
            var effectFile = "Assets/Effects/PreprocessorTest.fx";
            var effectCode = File.ReadAllText(effectFile);
            var fullPath = Path.GetFullPath(effectFile);

            // Preprocess.
            var mgDependencies = new List<string>();
            var mgPreprocessed = Preprocessor.Preprocess(effectCode, fullPath, new Dictionary<string, string>
            {
                { "TEST2", "1" }
            }, mgDependencies, new TestEffectCompilerOutput());

            Assert.That(mgDependencies, Has.Count.EqualTo(1));
            Assert.That(Path.GetFileName(mgDependencies[0]), Is.EqualTo("PreprocessorInclude.fxh"));

            Assert.That(mgPreprocessed, Does.Not.Contain("Foo"));
            Assert.That(mgPreprocessed, Does.Contain("Bar"));
            Assert.That(mgPreprocessed, Does.Not.Contain("Baz"));

            Assert.That(mgPreprocessed, Does.Contain("FOO"));
            Assert.That(mgPreprocessed, Does.Not.Contain("BAR"));

            // Check that we can actually compile this file.
            BuildEffect(effectFile, TargetPlatform.Windows);
        }

        private class TestEffectCompilerOutput : IEffectCompilerOutput
        {
            public void WriteWarning(string file, int line, int column, string message)
            {
                Console.WriteLine("Warning: {0}({1},{2}): {3}", file, line, column, message);
            }

            public void WriteError(string file, int line, int column, string message)
            {
                Console.WriteLine("Error: {0}({1},{2}): {3}", file, line, column, message);
            }
        }
#endif

        [Test]
        [TestCase("Assets/Effects/ParserTest.fx")]
        public void TestParser(string effectFile)
        {
            BuildEffect(effectFile, TargetPlatform.Windows);
        }

        [Test]
        public void TestDefines()
        {
            Assert.DoesNotThrow(() => BuildEffect("Assets/Effects/DefinesTest.fx", TargetPlatform.Windows, "MACRO_DEFINE_TEST=3"));
            Assert.Throws<InvalidContentException>(() =>
                BuildEffect("Assets/Effects/DefinesTest.fx", TargetPlatform.Windows, "MACRO_DEFINE_TEST=4"));
            Assert.Throws<InvalidContentException>(() =>
                BuildEffect("Assets/Effects/DefinesTest.fx", TargetPlatform.Windows));
            Assert.Throws<InvalidContentException>(() =>
                BuildEffect("Assets/Effects/DefinesTest.fx", TargetPlatform.Windows, "INVALID_SYNTAX;ANOTHER_MACRO;MACRO_DEFINE_TEST=3"));
        }

        [Test]
        [TestCase("Assets/Effects/Stock/AlphaTestEffect.fx")]
        [TestCase("Assets/Effects/Stock/BasicEffect.fx")]
        [TestCase("Assets/Effects/Stock/DualTextureEffect.fx")]
        [TestCase("Assets/Effects/Stock/EnvironmentMapEffect.fx")]
        [TestCase("Assets/Effects/Stock/SkinnedEffect.fx")]
        [TestCase("Assets/Effects/Stock/SpriteEffect.fx")]
        public void BuildStockEffect(string effectFile)
        {
            BuildEffect(effectFile, TargetPlatform.Windows);
        }

        [Test]
        [TestCase("Assets/Effects/HullShader.fx")]
        [TestCase("Assets/Effects/DomainShader.fx")]
        [TestCase("Assets/Effects/GeometryShader.fx")]
        public void BuildShaderStageEffec(string effectFile)
        {
            BuildEffect(effectFile, TargetPlatform.Windows);
        }

        private void BuildEffect(string effectFile, TargetPlatform targetPlatform, string defines = null)
        {
            var importerContext = new ImporterContext();
            var importer = new EffectImporter();
            var input = importer.Import(effectFile, importerContext);

            Assert.NotNull(input);

            var processorContext = new TestProcessorContext(targetPlatform, Path.ChangeExtension(effectFile, ".xnb"));
            var processor = new EffectProcessor { Defines = defines };
            var output = processor.Process(input, processorContext);

            Assert.NotNull(output);

            // TODO: Should we test the writer?
        }
    }
}
