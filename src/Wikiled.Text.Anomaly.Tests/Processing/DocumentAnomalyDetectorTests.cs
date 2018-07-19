using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Tests.Processing
{
    [TestFixture]
    public class DocumentAnomalyDetectorTests
    {
        private AnomalyFactory instance;

        private Document document;

        [SetUp]
        public void Setup()
        {
            document = Global.InitDocument("cv002_17424.txt");
            instance = new AnomalyFactory(Global.StyleFactory, Global.Dictionary);
        }

        [Test]
        public void AnomalyCosine()
        {
            var distances = instance.CreateSimple(document, false, 7);
            var result = distances.Detect(FilterTypes.Cosine);
            Assert.AreEqual(42, document.Sentences.Count);
            Assert.AreEqual(35, result.Sentences.Count);
            Assert.AreEqual(1, distances.Anomaly.Length);
            Assert.AreEqual(4, distances.Anomaly[0].Block.Length);
            Assert.AreEqual("Permission to make digital or hard copies of part or all of this work for personal or", distances.Anomaly[0].Block[0].Text);
        }

        [Test]
        public void AnomalyKMeans()
        {
            var distances = instance.CreateSimple(document, false, 1);
            var result = distances.Detect(FilterTypes.KMeans);
            Assert.AreEqual(42, document.Sentences.Count);
            Assert.AreEqual(34, result.Sentences.Count);
            Assert.AreEqual(1, distances.Anomaly.Length);
            Assert.AreEqual(10, distances.Anomaly[0].Block.Length);
            Assert.AreEqual("Permission to make digital or hard copies of part or all of this work for personal or", distances.Anomaly[0].Block[0].Text);
        }

        [Test]
        public void AnomalyKMeansRealDoc()
        {
            var document = JsonConvert.DeserializeObject<Document>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "doc.json")));
            var source = new DocumentVectorSource(Global.StyleFactory, Global.Dictionary, AnomalyVectorType.Full);
            var original = document.Sentences.Select(item => source.GetVector(new []{ item }, NormalizationType.None));
            var l2 = document.Sentences.Select(item => source.GetVector(new[] { item }, NormalizationType.L2));
            new JsonVectorSerialization(@"c:\1\vector.json").Serialize(original);
            new JsonVectorSerialization(@"c:\1\vector_l2.json").Serialize(l2);
            var distances = instance.CreateSimple(document);
            var result = distances.Detect(FilterTypes.KMeans);
            Assert.AreEqual(776, document.Sentences.Count);
            Assert.AreEqual(570, result.Sentences.Count);
            Assert.AreEqual(32, distances.Anomaly.Length);
            Assert.AreEqual(10, distances.Anomaly[0].Block.Length);
        }

        [Test]
        public void AnomalySentimentSmallSentiment()
        {
            var distances = instance.CreateSimple(document, true);
            document.Sentences[0].Words[0].CalculatedValue = 1;
            var result = distances.Detect(FilterTypes.Sentiment);
            Assert.AreEqual(42, document.Sentences.Count);
            Assert.AreEqual(1, result.Sentences.Count);
        }

        [Test]
        public void AnomalySentiment()
        {
            var distances = instance.CreateSimple(document, true);
            int sign = 2;
            for (int i = 0; i < 20; i++)
            {
                if (i % 3 == 0)
                {
                    sign = -sign;
                }

                document.Sentences[i].Words[0].CalculatedValue = sign;
            }
            
            var result = distances.Detect(FilterTypes.Sentiment);
            Assert.AreEqual(42, document.Sentences.Count);
            Assert.AreEqual(12, result.Sentences.Count);
        }
    }
}
