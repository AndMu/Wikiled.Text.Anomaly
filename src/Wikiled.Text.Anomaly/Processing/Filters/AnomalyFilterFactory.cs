using System;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class AnomalyFilterFactory : IAnomalyFilterFactory
    {
        private readonly ILogger<AnomalyFilterFactory> logger;

        private readonly ILoggerFactory loggerFactory;

        private readonly IDocumentVectorSource vectorSource;

        public AnomalyFilterFactory(ILoggerFactory loggerFactory, IDocumentVectorSource vectorSource)
        {
            this.loggerFactory = loggerFactory;
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            logger = loggerFactory.CreateLogger<AnomalyFilterFactory>();
        }

        public IAnomalyFilter Create(FilterTypes type)
        {
            logger.LogDebug("Create: {0}", type);
            switch (type)
            {
                case FilterTypes.Sentiment:
                    return new SentimentAnomalyFilter();
                case FilterTypes.Svm:
                    return new SvmAnomalyFilter(loggerFactory.CreateLogger< SvmAnomalyFilter>(), vectorSource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
