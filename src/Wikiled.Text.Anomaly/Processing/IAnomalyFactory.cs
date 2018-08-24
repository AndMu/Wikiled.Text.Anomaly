using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IAnomalyFactory
    {
        IDocumentAnomalyDetector CreateSimple(Document document, bool useSentimentClusters = false, int windowSize = 3);
    }
}