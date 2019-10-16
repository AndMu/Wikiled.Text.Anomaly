using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Tests.Processing
{
    [TestFixture]
    public class DocumentAnomalyDetectorTests
    {
        private AnomalyFactory instance;

        private ComplexDocument document;

        [SetUp]
        public void Setup()
        {
            document = new ComplexDocument(Global.InitDocument("cv002_17424.txt"));
            instance = new AnomalyFactory(new NullLoggerFactory(), new EmbeddingVectorSource(WordModel.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\model.bin"))));
        }

        [Test]
        public void AnomalySvm()
        {
            document = new ComplexDocument(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Svm);
            Assert.AreEqual(784, document.Sentences.Length);
            Assert.AreEqual(627, result.Document.Sentences.Count);
            Assert.AreEqual(28, result.Anomaly.Length);
            Assert.AreEqual(9, result.Anomaly[0].Sentences.Count);
            Assert.AreEqual("Kevin DiCiurcio, CFA Joshua M.", result.Anomaly[0].Sentences[0].Text);
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
            document = new ComplexDocument(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Svm, FilterTypes.Sentiment);
            Assert.AreEqual(784, document.Sentences.Length);
            Assert.AreEqual(601, result.Document.Sentences.Count);
            Assert.AreEqual(36, result.Anomaly.Length);
        }
    }
}
