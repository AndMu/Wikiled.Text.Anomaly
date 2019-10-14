using System.Collections.Generic;
using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class SentimentAnomalyFilter : IAnomalyFilter
    {
        public FilterTypes Type => FilterTypes.Sentiment;

        public DetectionResults Filter(DocumentClusters document)
        {
            var clusters = new List<IProcessingTextBlock>();
            var withoutSentiment = new List<IProcessingTextBlock>();
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
