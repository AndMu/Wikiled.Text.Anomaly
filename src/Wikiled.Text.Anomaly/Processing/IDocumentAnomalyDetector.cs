using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IDocumentAnomalyDetector
    {
        TextCluster[] Anomaly { get; }

        Document Document { get; }

        int MinimumSentencesCount { get; }

        bool UseSentimentClusters { get; }

        Document Detect(params FilterTypes[] types);
    }
}