using System;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Style.Logic;

namespace Wikiled.Text.Anomaly.Processing
{
    public class AnomalyFactory
    {
        private readonly IStyleFactory styleFactory;

        private readonly INRCDictionary dictionary;

        public AnomalyFactory(IStyleFactory styleFactory, INRCDictionary dictionary)
        {
            this.styleFactory = styleFactory ?? throw new ArgumentNullException(nameof(styleFactory));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public IDocumentAnomalyDetector Create(Document document)
        {
            throw new NotImplementedException();
            //return new DocumentAnomalyDetector(styleFactory, dictionary, document);
        }
    }
}
