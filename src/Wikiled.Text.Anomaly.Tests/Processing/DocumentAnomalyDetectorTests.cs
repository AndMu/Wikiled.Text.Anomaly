using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Anomaly.Processing.Vectors;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Tests.Processing
{
    [TestFixture]
    public class DocumentAnomalyDetectorTests
    {
        private AnomalyFactory instance;

        private DocumentBlock document;

        [SetUp]
        public void Setup()
        {
            document = new DocumentBlock(Global.InitDocument("cv002_17424.txt"));
            instance = new AnomalyFactory(new EmbeddingVectorSource(WordModel.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\model.bin"))));
        }

        [Test]
        public void AnomalySvm()
        {
            document = new DocumentBlock(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Svm);
            Assert.AreEqual(776, document.Sentences.Length);
            Assert.AreEqual(156, result.Document.Sentences.Count);
            Assert.AreEqual(30, result.Anomaly.Length);
            Assert.AreEqual(14, result.Anomaly[0].Sentences.Length);
            Assert.AreEqual("vanguard research december 2017 vanguard economic", result.Anomaly[0].Sentences[0].Text.Substring(0, 49));
        }

        [Test]
        public void AnomalySvmRealDoc()
        {
            document = new DocumentBlock(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var source = new DocumentVectorSource(Global.StyleFactory, Global.Dictionary, AnomalyVectorType.Full);
            List<VectorData> vectors = new List<VectorData>();
            for (int i = 0; i < document.Sentences.Length - 5; i++)
            {
                vectors.Add(source.GetVector(new ProcessingTextBlock(document.Sentences.Skip(i).Take(5).ToArray()), NormalizationType.None));
            }
            
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Svm);
            Assert.AreEqual(776, document.Sentences.Length);
            Assert.AreEqual(156, result.Document.Sentences.Count);
        }

        [Test]
        public void AnomalySentimentSmallSentiment()
        {
            var distances = instance.CreateSimple(document);
            document.Sentences[0].Words[0].CalculatedValue = 1;
            var result = distances.Detect(FilterTypes.Sentiment);
            Assert.AreEqual(42, document.Sentences.Length);
            Assert.AreEqual(1, result.Document.Sentences.Count);
        }

        [Test]
        public void AnomalySentiment()
        {
            var distances = instance.CreateSimple(document);
            for (int i = 0; i < 12; i++)
            {
                document.Sentences[i].Words[0].CalculatedValue = 2;
            }
            
            var result = distances.Detect(FilterTypes.Sentiment);
            Assert.AreEqual(42, document.Sentences.Length);
            Assert.AreEqual(12, result.Document.Sentences.Count);
        }

        [Test]
        public void AnomalySentimentDocument()
        {
            document = new DocumentBlock(JsonConvert.DeserializeObject<Document>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Sentiment, FilterTypes.Svm);
            Assert.AreEqual(776, document.Sentences.Length);
            Assert.AreEqual(72, result.Document.Sentences.Count);
        }
    }
}
