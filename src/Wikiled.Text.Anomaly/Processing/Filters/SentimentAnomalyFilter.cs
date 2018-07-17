using System.Collections.Generic;
using System.Linq;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class SentimentAnomalyFilter : IAnomalyFilter
    {
        public FilterTypes Type => FilterTypes.Sentiment;

        public DetectionResults Filter(DocumentClusters document)
        {
            List<TextCluster> clusters = new List<TextCluster>();
            List<TextCluster> withoutSentiment = new List<TextCluster>();
            foreach (var cluster in document.Clusters)
            {
                if (cluster.Block.Any(item => item.CalculateSentiment().HasValue))
                {
                    clusters.Add(cluster);
                }
                else
                {
                    withoutSentiment.Add(cluster);
                }
            }

            return new DetectionResults(withoutSentiment.ToArray(), clusters.ToArray());
        }
    }
}
