using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentResults
    {
        public DocumentResults(Document document, TextCluster[] clusters)
        {
            Clusters = clusters;
            Document = document;
        }

        public Document Document { get; }

        public TextCluster[]Clusters { get; }
    }
}
