using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IDocumentAnomalyDetector
    {
        DetectionResult Detect(params FilterTypes[] types);
    }
}