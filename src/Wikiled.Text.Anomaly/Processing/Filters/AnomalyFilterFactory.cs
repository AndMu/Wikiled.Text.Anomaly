using System;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class AnomalyFilterFactory : IAnomalyFilterFactory
    {
        private readonly ILogger<AnomalyFilterFactory> logger;

        private readonly IDocumentVectorSource vectorSource;

        public AnomalyFilterFactory(ILogger<AnomalyFilterFactory> logger, IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IAnomalyFilter Create(FilterTypes type)
        {
            logger.LogDebug("Create: {0}", type);
            switch (type)
            {
                case FilterTypes.Sentiment:
                    return new SentimentAnomalyFilter();
                case FilterTypes.Svm:
                    return new SvmAnomalyFilter(vectorSource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
