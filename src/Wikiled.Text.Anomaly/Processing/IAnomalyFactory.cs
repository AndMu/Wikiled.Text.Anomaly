using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IAnomalyFactory
    {
        IDocumentAnomalyDetector CreateSimple(ComplexDocument document, int windowSize = 3);
    }
}