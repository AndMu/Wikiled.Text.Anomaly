using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Clustering
{
    public class DocumentClusters
    {
        public DocumentClusters(Document document, TextCluster[] clusters)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Clusters = clusters ?? throw new ArgumentNullException(nameof(clusters));
        }

        public TextCluster[] Clusters { get; }

        public Document Document { get; }
    }
}
