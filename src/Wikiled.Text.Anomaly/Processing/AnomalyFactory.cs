using System;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Processing.Vectors;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalyFactory : IAnomalyFactory
    {
        private readonly IDocumentVectorSource documentVector;

        public AnomalyFactory(IDocumentVectorSource documentVector)
        {
            this.documentVector = documentVector ?? throw new ArgumentNullException(nameof(documentVector));
        }

        public IDocumentAnomalyDetector CreateSimple(Document document, bool useSentimentClusters = false, int windowSize = 3)
        {
            return new DocumentAnomalyDetector(
                document,
                new AnomalyFilterFactory(documentVector),
                new DocumentReconstructor(),
                windowSize);
        }
    }
}
