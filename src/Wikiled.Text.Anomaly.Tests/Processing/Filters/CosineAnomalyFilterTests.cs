using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Tests.Processing.Filters
{
    [TestFixture]
    public class CosineAnomalyFilterTests
    {
        private Mock<IDocumentVectorSource> mockDocumentVectorSource;

        private CosineAnomalyFilter instance;

        [SetUp]
        public void SetUp()
        {
            mockDocumentVectorSource = new Mock<IDocumentVectorSource>();
            instance = CreateInstance();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Filter(bool isDifferent)
        {
            Document document = new Document("Test");
            document.Sentences.Add(new SentenceItem("One"));
            document.Sentences.Add(new SentenceItem("Two"));
            document.Sentences.Add(new SentenceItem("Three"));
            document.Sentences.Add(new SentenceItem("Four"));
            TextCluster[] clusters = {
                new TextCluster(document.Sentences[0]),
                new TextCluster(document.Sentences[1]),
                new TextCluster(document.Sentences[2]),
                new TextCluster(document.Sentences[3])
            };

            var vector = new VectorDataFactory().CreateSimple(1, 0, 1);
            for (int i = 0; i < 4; i++)
            {
                var current = document.Sentences[i];
                var other = document.Sentences.Where(item => item != current).ToArray();
                var wordVector = vector;
                if (i == 3 && isDifferent)
                {
                    wordVector = new VectorDataFactory().CreateSimple(0.5, 0, 0);
                }

                mockDocumentVectorSource.Setup(item =>
                    item.GetVector(new[] { current }, NormalizationType.L2))
                    .Returns(wordVector);
                
                mockDocumentVectorSource.Setup(item =>
                        item.GetVector(other, NormalizationType.L2))
                    .Returns(vector);
            }

            var result = instance.Filter(new DocumentClusters(document, clusters));
            if (isDifferent)
            {
                Assert.AreEqual(3, result.Result.Length);
                Assert.AreEqual(1, result.Anomaly.Length);
                Assert.IsFalse(result.Result.Contains(clusters[3]));
            }
            else
            {
                Assert.AreEqual(4, result.Result.Length);
            }
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new CosineAnomalyFilter(null));
        }

        private CosineAnomalyFilter CreateInstance()
        {
            return new CosineAnomalyFilter(mockDocumentVectorSource.Object);
        }
    }
}
