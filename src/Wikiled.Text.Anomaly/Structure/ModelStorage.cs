using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.Common.Extensions;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class ModelStorage<T> : IModelStorage<T>
        where T : class, IModel
    {
        private readonly ILogger<ModelStorage<T>> logger;

        private List<Document> negative = new List<Document>();

        private List<Document> positive = new List<Document>();

        private readonly Dictionary<string, Document> duplicate = new Dictionary<string, Document>(StringComparer.OrdinalIgnoreCase);

        private T current;

        private readonly IDocumentReconstructor reconstructor;

        private readonly IModelFactory<T> factory;

        public ModelStorage(ILogger<ModelStorage<T>> logger, IDocumentReconstructor reconstructor, IModelFactory<T> factory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public T Current => current;

        public void Reset()
        {
            logger.LogDebug("Reset");
            negative.Clear();
            positive.Clear();
            duplicate.Clear();
            current = null;
        }

        public void Add(DataType type, params IProcessingTextBlock[] blocks)
        {
            logger.LogDebug("Add: {0}", type);
            IEnumerable<Document> documents = blocks.Select(item => reconstructor.Reconstruct(item.Sentences));
            foreach (var document in documents)
            {
                if (string.IsNullOrEmpty(document.Text))
                {
                    logger.LogWarning("Ignoring empty document");
                    continue;
                }

                if (duplicate.ContainsKey(document.Text))
                {
                    logger.LogWarning("Duplicate document detected - ignoring");
                    continue;
                }

                duplicate[document.Text] = document;
                if (type == DataType.Positive)
                {
                    positive.Add(document);
                }
                else
                {
                    negative.Add(document);
                }
            }
        }

        public T Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            logger.LogInformation("Loading <{0}>...", path);
            (string postiveFile, string negativeFile, string modelFile) files = GetFiles(path);

            if (File.Exists(files.postiveFile))
            {
                logger.LogDebug("Loading <{0}> positive documents", files.postiveFile);
                Document[] positiveDocs = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(files.postiveFile));
                positive = GetDocuments(positiveDocs);
            }

            if (File.Exists(files.negativeFile))
            {
                logger.LogDebug("Loading <{0}> negative documents", files.negativeFile);
                Document[] negativeDocs = JsonConvert.DeserializeObject<Document[]>(File.ReadAllText(files.negativeFile));
                negative = GetDocuments(negativeDocs);
            }

            current = factory.Load(path);
            return Current;
        }

        public void Save(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            logger.LogInformation("Saving <{0}>...", path);
            path.EnsureDirectoryExistence();

            (string postiveFile, string negativeFile, string modelFile) files = GetFiles(path);
            File.WriteAllText(files.postiveFile, JsonConvert.SerializeObject(positive.ToArray()));
            File.WriteAllText(files.negativeFile, JsonConvert.SerializeObject(negative.ToArray()));
            factory.Save(path, Current);
        }


        public async Task<T> Train(CancellationToken token)
        {
            logger.LogDebug("Train");
            if (positive.Count < 5 || negative.Count < 5)
            {
                throw new InvalidOperationException("Not enough training samples");
            }

            var instance = factory.CreateNew();
            var dataset = new DataSet
            {
                Positive = positive.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray(),
                Negative = negative.Select(item => new ProcessingTextBlock(item.Sentences.ToArray())).ToArray()
            };

            await instance.Train(dataset, token).ConfigureAwait(false);
            current = instance;
            return instance;
        }

        private static (string postiveFile, string negativeFile, string modelFile) GetFiles(string path)
        {
            string positiveFile = Path.Combine(path, "positive.json");
            string negativeFile = Path.Combine(path, "negative.json");
            string modelFile = Path.Combine(path, "model.dat");
            return (positiveFile, negativeFile, modelFile);
        }

        private List<Document> GetDocuments(Document[] documents)
        {
            var list = new List<Document>(documents);
            foreach (var doc in documents)
            {
                duplicate[doc.Text] = doc;
            }

            return list;
        }
    }
}
