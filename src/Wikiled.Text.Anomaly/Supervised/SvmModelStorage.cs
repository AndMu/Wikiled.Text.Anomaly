using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.Common.Extensions;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class SvmModelStorage : IModelStorage
    {
        private readonly ILoggerFactory factory;

        private readonly ILogger logger;

        private readonly IDocumentReconstructor reconstructor;

        private readonly IDocumentVectorSource vectorSource;

        private List<Document> negative = new List<Document>();

        private List<Document> positive = new List<Document>();

        private SvmAnomalyDetector current;

        public SvmModelStorage(ILoggerFactory factory, IDocumentVectorSource vectorSource, IDocumentReconstructor reconstructor)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            logger = factory.CreateLogger<SvmModelStorage>();
        }

        public IAnomalyDetector Current => current;

        public void Add(DataType type, params IProcessingTextBlock[] blocks)
        {
            logger.LogDebug("Add: {0}", type);
            IEnumerable<Document> documents = blocks.Select(item => reconstructor.Reconstruct(item.Sentences));
            if(type == DataType.Positive)
            {
                positive.AddRange(documents);
            }
            else
            {
                negative.AddRange(documents);
            }
        }

        public IAnomalyDetector Load(string path)
        {
            logger.LogInformation("Loading <{0}>...", path);
            var files = GetFiles(path);

            if(File.Exists(files.postiveFile))
            {
                logger.LogDebug("Loading <{0}> positive documents", files.postiveFile);
                Document[] positiveDocs = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(files.postiveFile));
                positive = new List<Document>(positiveDocs);
            }

            if(File.Exists(files.negativeFile))
            {
                logger.LogDebug("Loading <{0}> negative documents", files.negativeFile);
                Document[] negativeDocs = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(files.negativeFile));
                negative = new List<Document>(negativeDocs);
            }

            SupportVectorMachine<Linear> model = null;
            if(File.Exists(files.modelFile))
            {
                logger.LogDebug("Loading <{0}> model", files.modelFile);
                model = Serializer.Load<SupportVectorMachine<Linear>>(files.modelFile);
            }

            logger.LogDebug("Loaded <{0}> positive and <{1}> negative", positive.Count, negative.Count);
            current = new SvmAnomalyDetector(vectorSource, factory, model);
            return Current;
        }

        public void Save(string path)
        {
            logger.LogInformation("Saving <{0}>...", path);
            path.EnsureDirectoryExistence();
            if(current?.Model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            var files = GetFiles(path);
            File.WriteAllText(files.postiveFile, JsonConvert.SerializeObject(positive.ToArray()));
            File.WriteAllText(files.negativeFile, JsonConvert.SerializeObject(negative.ToArray()));
            current.Model.Save(files.modelFile);
        }

        public async Task<SvmAnomalyDetector> Train(CancellationToken token)
        {
            logger.LogDebug("Train");
            if(positive.Count < 5 || negative.Count < 5)
            {
                throw new InvalidOperationException("Not enough training samples");
            }

            SvmAnomalyDetector detector = new SvmAnomalyDetector(vectorSource, factory, null);
            DataSet dataset = new DataSet
                              {
                                  Positive = positive.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray(),
                                  Negative = negative.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray()
                              };
            await detector.Train(dataset, token).ConfigureAwait(false);
            current = detector;
            return detector;
        }

        private static (string postiveFile, string negativeFile, string modelFile) GetFiles(string path)
        {
            string positiveFile = Path.Combine(path, "positive.json");
            var negativeFile = Path.Combine(path, "negative.json");
            var modelFile = Path.Combine(path, "model.dat");
            return (positiveFile, negativeFile, modelFile);
        }
    }
}
