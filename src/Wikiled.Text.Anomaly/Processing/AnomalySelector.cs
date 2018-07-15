using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalySelector
    {
        private IDocumentVectorSource vectorSource;

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        public SentenceItem[] Filter(IEnumerable<SentenceItem[]> sentenceClusters)
        {
            ConcurrentBag<IItemProbability<SentenceItem[]>> list = new ConcurrentBag<IItemProbability<SentenceItem[]>>();
            Parallel.ForEach(
                sentenceClusters,
                segment =>
                {
                    if (segment.Length == 0)
                    {
                        return;
                    }

                    VectorData currentVector = vectorSource.GetVector(segment, NormalizationType.L2);
                    var remainingSentences = sentences.Where(item => !segment.Contains(item)).ToArray();
                    if (remainingSentences.Length == 0)
                    {
                        return;
                    }

                    VectorData remainingVector = vectorSource.GetVector(remainingSentences, NormalizationType.L2);
                    double distance = distanceLogic.Measure(currentVector, remainingVector);
                    list.Add(
                        new ItemProbability<SentenceItem[]>(segment)
                        {
                            Probability = Math.Abs(distance)
                        });
                });
        }
    }
}
