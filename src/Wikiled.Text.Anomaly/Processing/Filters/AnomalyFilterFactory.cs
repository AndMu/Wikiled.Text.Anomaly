using System;
using NLog;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class AnomalyFilterFactory
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentVectorSource vectorSource;

        public AnomalyFilterFactory(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
        }

        public IAnomalyFilter Create(FilterTypes type)
        {
            logger.Debug("Create: {0}", type);
            switch (type)
            {
                case FilterTypes.Sentiment:
                    return new SentimentAnomalyFilter();
                case FilterTypes.KMeans:
                    return new KmeanAnomalyFilter(vectorSource);
                case FilterTypes.Cosine:
                    return new CosineAnomalyFilter(vectorSource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
