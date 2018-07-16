using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class CosineAnomalyFilter : IAnomalyFilter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentVectorSource vectorSource;

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        public CosineAnomalyFilter(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
        }

        public FilterTypes Type => FilterTypes.KMeans;

        public TextCluster[] Filter(DocumentClusters document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            ConcurrentBag<IItemProbability<TextCluster>> list = new ConcurrentBag<IItemProbability<TextCluster>>();
            Parallel.ForEach(
                document.Clusters,
                segment =>
                {
                    if (segment.Block.Length == 0)
                    {
                        return;
                    }

                    VectorData currentVector = vectorSource.GetVector(segment.Block, NormalizationType.L2);
                    var remainingSentences = document.Document.Sentences.Where(item => !segment.Contains(item)).ToArray();
                    if (remainingSentences.Length == 0)
                    {
                        return;
                    }

                    VectorData remainingVector = vectorSource.GetVector(remainingSentences, NormalizationType.L2);
                    double distance = distanceLogic.Measure(currentVector, remainingVector);
                    list.Add(
                        new ItemProbability<TextCluster>(segment)
                        {
                            Probability = Math.Abs(distance)
                        });
                });

            var sorted = list.OrderBy(item => item.Probability).ToList();
            var difference = sorted[sorted.Count / 2].Probability - sorted[0].Probability;
            int anomalyCount = 0;
            if (difference > 0.05)
            {
                anomalyCount = 1;
                for (int i = 1; i < sorted.Count / 2; i++)
                {
                    var distanceToAnomaly = sorted[i].Probability - sorted[0].Probability;
                    var distanceToGood = sorted[sorted.Count / 2].Probability - sorted[i].Probability;
                    if (distanceToAnomaly < distanceToGood)
                    {
                        anomalyCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                logger.Info("Difference is too small. No anomaly detected");
            }

            return sorted.Skip(anomalyCount).Select(item => item.Data).ToArray();
        }
    }
}
