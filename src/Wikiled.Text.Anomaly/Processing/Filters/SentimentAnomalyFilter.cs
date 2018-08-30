using System.Collections.Generic;
using System.Linq;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class SentimentAnomalyFilter : IAnomalyFilter
    {
        public FilterTypes Type => FilterTypes.Sentiment;

        public DetectionResults Filter(DocumentClusters document)
        {
            List<ProcessingTextBlock> clusters = new List<ProcessingTextBlock>();
            List<ProcessingTextBlock> withoutSentiment = new List<ProcessingTextBlock>();
            foreach (var cluster in document.Clusters)
            {
                if (cluster.Sentences.Any(item => item.CalculateSentiment().HasValue))
                {
                    clusters.Add(cluster);
                }
                else
                {
                    withoutSentiment.Add(cluster);
                }
            }

            return new DetectionResults(clusters.ToArray(), withoutSentiment.ToArray());
        }
    }
}
