using System.Collections.Generic;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IDocumentVectorSource
    {
        AnomalyVectorType AnomalyVectorType { get; }

        VectorData GetDocumentVector(Document document, NormalizationType normalization);

        VectorData GetVector(IEnumerable<SentenceItem> normalBlock, NormalizationType normalization);
    }
}