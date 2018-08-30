using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Processing.Filters;
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
            Assert.AreEqual(784, document.Sentences.Length);
            Assert.AreEqual(627, result.Document.Sentences.Count);
            Assert.AreEqual(28, result.Anomaly.Length);
            Assert.AreEqual(9, result.Anomaly[0].Sentences.Length);
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
            document = new DocumentBlock(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.Svm, FilterTypes.Sentiment);
            Assert.AreEqual(784, document.Sentences.Length);
            Assert.AreEqual(601, result.Document.Sentences.Count);
            Assert.AreEqual(36, result.Anomaly.Length);
        }
    }
}
