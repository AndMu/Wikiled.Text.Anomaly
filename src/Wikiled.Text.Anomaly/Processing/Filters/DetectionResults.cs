using System;
using System.Linq;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class DetectionResults
    {
        public DetectionResults(TextCluster[] result)
            : this(new TextCluster[] { }, result)
        {
        }

        public DetectionResults(TextCluster[] anomaly, TextCluster[] result)
        {
            Anomaly = anomaly ?? throw new ArgumentNullException(nameof(anomaly));
            Result = result ?? throw new ArgumentNullException(nameof(result));
            var anomalyLookup = Anomaly.SelectMany(item => item.Block).ToLookup(item => item);
            for (int i = 0; i < Result.Length; i++)
            {
                Result[i] = new TextCluster(Result[i].Block.Where(item => !anomalyLookup.Contains(item)).ToArray());
            }
        }

        public TextCluster[] Anomaly { get; }

        public TextCluster[] Result { get; }
    }
}
