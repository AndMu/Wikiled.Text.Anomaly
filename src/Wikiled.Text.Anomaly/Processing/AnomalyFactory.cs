using System;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Model;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalyFactory : IAnomalyFactory
    {
        private readonly IDocumentVectorSource documentVector;

        public AnomalyFactory(IDocumentVectorSource documentVector)
        {
            this.documentVector = documentVector ?? throw new ArgumentNullException(nameof(documentVector));
        }

        public IDocumentAnomalyDetector CreateSimple(ComplexDocument document, int windowSize = 3)
        {
            return new DocumentAnomalyDetector(
                document,
                new AnomalyFilterFactory(documentVector),
                new DocumentReconstructor(),
                windowSize);
        }
    }
}
