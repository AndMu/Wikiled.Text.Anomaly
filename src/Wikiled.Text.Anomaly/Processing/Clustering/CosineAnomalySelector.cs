using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Clustering
{
    public class CosineAnomalySelector
    {
        private readonly IDocumentVectorSource vectorSource;

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        public CosineAnomalySelector(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource;
        }

        public SentenceItem[] Filter(DocumentClusters document)
        {
            ConcurrentBag<IItemProbability<SentenceItem[]>> list = new ConcurrentBag<IItemProbability<SentenceItem[]>>();
            Parallel.ForEach(
                document.Clusters,
                segment =>
                {
                    if (segment.Block.Length == 0)
                    {
                        return;
                    }

                    VectorData currentVector = vectorSource.GetVector(segment.Block, NormalizationType.L2);
                    var remainingSentences = document.Document.Sentences.Where(item => !segment.Block.Contains(item)).ToArray();
                    if (remainingSentences.Length == 0)
                    {
                        return;
                    }

                    VectorData remainingVector = vectorSource.GetVector(remainingSentences, NormalizationType.L2);
                    double distance = distanceLogic.Measure(currentVector, remainingVector);
                    list.Add(
                        new ItemProbability<SentenceItem[]>(segment.Block)
                        {
                            Probability = Math.Abs(distance)
                        });
                });

            var sorted = list.OrderBy(item => item.Probability).ToList();
            int anomalyCount = 1;
            for (int i = 1; i < sorted.Count / 2; i++)
            {
                var distanceToAnomaly = sorted[1].Probability - sorted[0].Probability;
                var distanceToGood = sorted[1].Probability - sorted[sorted.Count / 2].Probability;
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
    }
}
