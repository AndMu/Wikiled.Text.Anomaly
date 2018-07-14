using Wikiled.MachineLearning.Clustering;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IDocumentAnomalyDetector
    {
        SentenceItem[] Anomaly { get; }

        int AnomalySentencesCount { get; }

        double AnomalyThreshold { get; set; }

        AnomalyVectorType AnomalyVectorType { get; set; }

        Document Document { get; }

        int MinimumSentencesCount { get; }

        double MinimumWordsCount { get; }

        VectorData RemainingVector { get; set; }

        bool UseSentimentClusters { get; set; }

        bool UseVector { get; set; }

        double WindowSize { get; set; }

        SentenceItem[] WithoutAnomaly { get; }

        void Detect();

        SentenceItem[] GetData(ClusterRegion region);

        VectorData GetDocumentVector(NormalizationType normalization);

        bool IsInAnomaly(SentenceItem sentence);

        void Reset();
    }
}