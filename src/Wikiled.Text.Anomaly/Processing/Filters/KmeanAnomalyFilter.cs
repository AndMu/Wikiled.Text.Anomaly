using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.MachineLearning;
using NLog;
using Wikiled.MachineLearning.Normalization;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class KmeanAnomalyFilter : IAnomalyFilter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentVectorSource vectorSource;

        public KmeanAnomalyFilter(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
        }

        public FilterTypes Type => FilterTypes.KMeans;

        public DetectionResults Filter(DocumentClusters document)
        {
            if (document.Clusters.Length < 3)
            {
                logger.Info("Not enought text clusters for clustering");
                return new DetectionResults(document.Clusters);
            }

            double[][] observations = new double[document.Clusters.Length][];
            Parallel.For(0,
                         document.Clusters.Length,
                         i =>
                         {
                             var documentCluster = document.Clusters[i];
                             var result = vectorSource.GetVector(documentCluster.Block, NormalizationType.L2).Values;
                             observations[i] = result;
                         });
          
            var data = observations.ToArray();
            var clusterNumber = document.Clusters.Length > 5 ? 5 : document.Clusters.Length;
            KMeans kmeans = new KMeans(clusterNumber);
            KMeansClusterCollection clusters = kmeans.Learn(data);
            int[] labels = clusters.Decide(data);
            Dictionary<int, int> occurences = new Dictionary<int, int>();
            Dictionary<int, List<int>> occurenceIndexes = new Dictionary<int, List<int>>();
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                occurences.TryGetValue(label, out var current);
                current++;
                occurences[label] = current;
                if (!occurenceIndexes.TryGetValue(label, out var list))
                {
                    list = new List<int>();
                    occurenceIndexes[label] = list;
                }

                list.Add(i);
            }

            if (occurences.Count == 1)
            {
                logger.Info("No anomaly found");
                return new DetectionResults(document.Clusters);
            }

            var anomalyLabel = occurences.OrderBy(item => item.Value).First();
            var exclude = occurenceIndexes[anomalyLabel.Key];
            List<TextCluster> finalResult = new List<TextCluster>();
            List<TextCluster> anomaly = new List<TextCluster>();
            for (int i = 0; i < document.Clusters.Length; i++)
            {
                if (!exclude.Contains(i))
                {
                    finalResult.Add(document.Clusters[i]);
                }
                else
                {
                    anomaly.Add(document.Clusters[i]);
                }
            }

            return new DetectionResults(anomaly.ToArray(), finalResult.ToArray());
        }
    }
}
