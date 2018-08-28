using System;

namespace Wikiled.Text.Anomaly.Structure
{
    public class DocumentClusters
    {
        public DocumentClusters(ProcessingTextBlock[] clusters)
        {
            Clusters = clusters ?? throw new ArgumentNullException(nameof(clusters));
        }

        public ProcessingTextBlock[] Clusters { get; }
    }
}
