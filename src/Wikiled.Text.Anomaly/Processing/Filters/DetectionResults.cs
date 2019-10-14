using System;
using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class DetectionResults
    {
        public DetectionResults(IProcessingTextBlock[] result)
            : this(Array.Empty<IProcessingTextBlock>(), result)
        {
        }

        public DetectionResults(IProcessingTextBlock[] result, IProcessingTextBlock[] anomaly)
        {
            Anomaly = anomaly ?? throw new ArgumentNullException(nameof(anomaly));
            Result = result ?? throw new ArgumentNullException(nameof(result));
            var anomalyLookup = Anomaly.SelectMany(item => item.Sentences).ToLookup(item => item);
            for (int i = 0; i < Result.Length; i++)
            {
                Result[i] = new ProcessingTextBlock(Result[i].Sentences.Where(item => !anomalyLookup.Contains(item)).ToArray());
            }
        }

        public IProcessingTextBlock[] Anomaly { get; }

        public IProcessingTextBlock[] Result { get; }
    }
}
