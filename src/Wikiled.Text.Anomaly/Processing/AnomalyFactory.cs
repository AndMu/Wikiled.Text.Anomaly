using System;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Style.Logic;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalyFactory : IAnomalyFactory
    {
        private readonly IStyleFactory styleFactory;

        private readonly INRCDictionary dictionary;

        public AnomalyFactory(IStyleFactory styleFactory, INRCDictionary dictionary)
        {
            this.styleFactory = styleFactory ?? throw new ArgumentNullException(nameof(styleFactory));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public IDocumentAnomalyDetector CreateSimple(Document document, bool useSentimentClusters = false, int minimumSentences = 3)
        {
            return new DocumentAnomalyDetector(document,
                new AnomalyFilterFactory(new DocumentVectorSource(styleFactory, dictionary, AnomalyVectorType.Full)),
                new DocumentReconstructor(),
                useSentimentClusters);
        }
    }
}
