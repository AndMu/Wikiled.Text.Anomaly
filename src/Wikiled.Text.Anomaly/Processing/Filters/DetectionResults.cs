using System;
using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class DetectionResults
    {
        public DetectionResults(ProcessingTextBlock[] result)
            : this(new ProcessingTextBlock[] { }, result)
        {
        }

        public DetectionResults(ProcessingTextBlock[] result, ProcessingTextBlock[] anomaly)
        {
            Anomaly = anomaly ?? throw new ArgumentNullException(nameof(anomaly));
            Result = result ?? throw new ArgumentNullException(nameof(result));
            var anomalyLookup = Anomaly.SelectMany(item => item.Sentences).ToLookup(item => item);
            for (int i = 0; i < Result.Length; i++)
            {
                Result[i] = new ProcessingTextBlock(Result[i].Sentences.Where(item => !anomalyLookup.Contains(item)).ToArray());
            }
        }

        public ProcessingTextBlock[] Anomaly { get; }

        public ProcessingTextBlock[] Result { get; }
    }
}
