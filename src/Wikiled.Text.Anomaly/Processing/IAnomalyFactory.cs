using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IAnomalyFactory
    {
        IDocumentAnomalyDetector CreateSimple(DocumentBlock document, int windowSize = 3);
    }
}