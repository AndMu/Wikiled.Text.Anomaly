using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Anomaly.Processing.Vectors;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Tests.Processing.Specific
{
    [TestFixture]
    public class TextBlockAnomalyDetectorTests
    {
        private IDocumentVectorSource vectorSource;

        private TextBlockAnomalyDetector<PageBlock> pageDetector;

        private DocumentBlock document;

        [SetUp]
        public void SetUp()
        {
            GlobalSettings.Random = new Random(48);
            vectorSource = new EmbeddingVectorSource(WordModel.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\model.bin")));
            pageDetector = new TextBlockAnomalyDetector<PageBlock>(vectorSource, new NullLoggerFactory());
            document = new DocumentBlock(JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json"))));
        }

        [Test]
        public async Task TestPages()
        {
            DataSet<PageBlock> dataSet = new DataSet<PageBlock>();
            dataSet.Negative = document.Pages.Take(2).Concat(document.Pages.Skip(30)).ToArray();
            dataSet.Positive = document.Pages.Skip(2).Take(29).ToArray();
            await pageDetector.Train(dataSet, CancellationToken.None).ConfigureAwait(false);
            var result = pageDetector.Predict(document.Pages).Select(item => item ? 1 : 0).ToArray();
            var expected = new int[document.Pages.Length];
            for (int i = 2; i <= 30; i++)
            {
                expected[i] = 1;
            }

            var cm = new GeneralConfusionMatrix(2, expected: expected, predicted: result);
            Assert.GreaterOrEqual(cm.PerClassMatrices[0].FScore, 0.8);
            Assert.GreaterOrEqual(cm.PerClassMatrices[1].FScore, 0.9);

            pageDetector.Save("model.dat");
            pageDetector = new TextBlockAnomalyDetector<PageBlock>(vectorSource, new NullLoggerFactory());
            pageDetector.Load("model.dat");
            result = pageDetector.Predict(document.Pages).Select(item => item ? 1 : 0).ToArray();
            cm = new GeneralConfusionMatrix(2, expected: expected, predicted: result);
            Assert.GreaterOrEqual(cm.PerClassMatrices[0].FScore, 0.8);
            Assert.GreaterOrEqual(cm.PerClassMatrices[1].FScore, 0.9);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new TextBlockAnomalyDetector<PageBlock>(null, new NullLoggerFactory()));
            Assert.Throws<ArgumentNullException>(() => new TextBlockAnomalyDetector<PageBlock>(vectorSource, null));
        }
    }
}
