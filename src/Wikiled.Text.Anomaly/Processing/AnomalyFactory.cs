using System;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalyFactory : IAnomalyFactory
    {
        private readonly IDocumentVectorSource documentVector;

        private readonly ILoggerFactory loggerFactory;

        public AnomalyFactory(ILoggerFactory loggerFactory, IDocumentVectorSource documentVector)
        {
            this.documentVector = documentVector ?? throw new ArgumentNullException(nameof(documentVector));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IDocumentAnomalyDetector CreateSimple(ComplexDocument document, int windowSize = 3)
        {
            return new DocumentAnomalyDetector(
                loggerFactory.CreateLogger<DocumentAnomalyDetector>(),
                document,
                new AnomalyFilterFactory(loggerFactory, documentVector),
                new DocumentReconstructor(),
                windowSize);
        }
    }
}
