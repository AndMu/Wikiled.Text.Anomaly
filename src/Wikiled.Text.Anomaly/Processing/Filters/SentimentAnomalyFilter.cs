using System;
using System.Collections.Generic;
using System.Linq;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class SentimentAnomalyFilter : IAnomalyFilter
    {
        public FilterTypes Type => FilterTypes.Sentiment;

        public TextCluster[] Filter(DocumentClusters document)
        {
            List<TextCluster> clusters = new List<TextCluster>();
            foreach (var cluster in document.Clusters)
            {
                if (cluster.Block.Any(item => item.CalculateSentiment().h > 0))
                {
                    clusters.Add(cluster);
                }
            }

            return clusters.ToArray();
        }
    }
}
