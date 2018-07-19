using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IAnomalyFactory
    {
        IDocumentAnomalyDetector CreateSimple(Document document, bool useSentimentClusters = false, double windowSize = 0.1);
    }
}