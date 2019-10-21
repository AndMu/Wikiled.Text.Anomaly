using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Supervised;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Tests.Supervised
{
    [TestFixture]
    public class SvmAnomalyDetectorTests
    {
        private IDocumentVectorSource vectorSource;

        private SvmAnomalyDetector pageDetector;

        private ComplexDocument document;

        [SetUp]
        public void SetUp()
        {
            GlobalSettings.Random = new Random(48);
            vectorSource = new EmbeddingVectorSource(WordModel.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\model.bin")));
            pageDetector = new SvmAnomalyDetector(new NullLoggerFactory(), vectorSource, null);
            document = new ComplexDocument(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
        }

        [Test]
        public async Task TestPages()
        {
            var dataSet = new DataSet();
            dataSet.Negative = document.Pages.Take(2).Concat(document.Pages.Skip(30)).ToArray();
            dataSet.Positive = document.Pages.Skip(2).Take(29).ToArray();
            await pageDetector.Train(dataSet, CancellationToken.None).ConfigureAwait(false);
            var result = pageDetector.Predict(document.Pages).Select(item => item ? 1 : 0).ToArray();
            var expected = new int[document.Pages.Length];
            for (int i = 2; i <= 30; i++)
            {
                expected[i] = 1;
            }

            var cm = new GeneralConfusionMatrix(2, expected, result);
            Assert.GreaterOrEqual(cm.PerClassMatrices[0].FScore, 0.8);
            Assert.GreaterOrEqual(cm.PerClassMatrices[1].FScore, 0.9);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SvmAnomalyDetector(new NullLoggerFactory(), null, null));
            Assert.Throws<ArgumentNullException>(() => new SvmAnomalyDetector(null, vectorSource, null));
        }
    }
}
