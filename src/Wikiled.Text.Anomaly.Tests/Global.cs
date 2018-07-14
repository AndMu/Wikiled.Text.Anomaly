using System.IO;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;
using Wikiled.Text.Analysis.Words;
using Wikiled.Text.Inquirer.Logic;
using Wikiled.Text.Style.Logic;

namespace Wikiled.Text.Anomaly.Tests
{
    [SetUpFixture]
    public class Global
    {
        public static NaivePOSTagger PosTagger { get; private set; }

        public static StyleFactory StyleFactory { get; private set; }

        public static SimpleWordsExtraction Extraction { get; private set; }

        public static NRCDictionary Dictionary { get; private set; }

        [OneTimeSetUp]
        public void Setup()
        {
            PosTagger = new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance);
            Dictionary = new NRCDictionary();
            Dictionary.Load();
            var inquirer = new InquirerManager();
            inquirer.Load();
            StyleFactory = new StyleFactory(PosTagger, new NRCDictionary(), new FrequencyListManager(), inquirer);
            var factory = new SentenceTokenizerFactory(PosTagger, new RawWordExtractor(new BasicEnglishDictionary(), new MemoryCache(new MemoryCacheOptions())));
            Extraction = new SimpleWordsExtraction(factory.Create(true, false));
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }

        public static Document InitDocument(string name = "cv000_29416.txt")
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data");
            return Extraction.GetDocument(File.ReadAllText(Path.Combine(path, name)));
        }
    }
}
