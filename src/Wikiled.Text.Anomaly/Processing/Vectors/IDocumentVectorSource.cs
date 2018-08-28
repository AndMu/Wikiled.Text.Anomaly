using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing.Vectors
{
    public interface IDocumentVectorSource
    {
        VectorData GetVector(IProcessingTextBlock textBlock, NormalizationType normalization);
    }
}