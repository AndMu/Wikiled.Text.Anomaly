using System;
using NLog;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class AnomalyFilterFactory : IAnomalyFilterFactory
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
                case FilterTypes.Svm:
                    return new SvmAnomalyFilter(vectorSource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
