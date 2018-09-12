using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Supervised;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Tests.Supervised
{
    [TestFixture]
    public class SvmModelStorageTests
    {
        private ILoggerFactory loggerFactory;

        private IDocumentVectorSource vectorSource;

        private IDocumentReconstructor documentReconstructor;

        private SvmModelStorage instance;

        private Document[] documents;

        [SetUp]
        public void SetUp()
        {
            loggerFactory = new NullLoggerFactory();
            vectorSource = new EmbeddingVectorSource(WordModel.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\model.bin")));
            documentReconstructor = new DocumentReconstructor();
            documents = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "docs.json")));
            instance = CreateModelStorage();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SvmModelStorage(
                null,
                vectorSource,
                documentReconstructor));
            Assert.Throws<ArgumentNullException>(() => new SvmModelStorage(
                loggerFactory,
                null,
                documentReconstructor));
            Assert.Throws<ArgumentNullException>(() => new SvmModelStorage(
                loggerFactory,
                vectorSource,
                null));
        }

        [Test]
        public async Task LogicTest()
        {
            var negative = documents.Take(2).Concat(documents.Skip(30)).Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray();
            var positive = documents.Skip(2).Take(29).Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray();
            instance.Add(DataType.Positive, positive);
            instance.Add(DataType.Negative, negative);
            var pageDetector = await instance.Train(CancellationToken.None).ConfigureAwait(false);

            var result = pageDetector.Predict(new DocumentBlock(documents).Pages).Select(item => item ? 1 : 0).ToArray();
            var expected = new int[documents.Length];
            for (int i = 2; i <= 30; i++)
            {
                expected[i] = 1;
            }

            var cm = new GeneralConfusionMatrix(2, expected: expected, predicted: result);
            Assert.GreaterOrEqual(cm.PerClassMatrices[0].FScore, 0.8);
            Assert.GreaterOrEqual(cm.PerClassMatrices[1].FScore, 0.9);

            instance.Save("Result");
            SvmModelStorageFactory storageFactory = new SvmModelStorageFactory(
                loggerFactory,
                vectorSource,
                documentReconstructor,
                new StorageConfig { Location = TestContext.CurrentContext.TestDirectory });
            instance = (SvmModelStorage)storageFactory.Construct("Result");
            result = pageDetector.Predict(new DocumentBlock(documents).Pages).Select(item => item ? 1 : 0).ToArray();
            cm = new GeneralConfusionMatrix(2, expected: expected, predicted: result);
            Assert.GreaterOrEqual(cm.PerClassMatrices[0].FScore, 0.8);
            Assert.GreaterOrEqual(cm.PerClassMatrices[1].FScore, 0.9);
        }

        private SvmModelStorage CreateModelStorage()
        {
            return new SvmModelStorage(
                loggerFactory,
                vectorSource,
                documentReconstructor);
        }
    }
}
