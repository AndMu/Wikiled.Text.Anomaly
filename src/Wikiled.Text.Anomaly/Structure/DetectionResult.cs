using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class DetectionResult
    {
        public DetectionResult(Document document, IProcessingTextBlock[] anomaly)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Anomaly = anomaly ?? throw new ArgumentNullException(nameof(anomaly));
        }

        public Document Document { get; }

        public IProcessingTextBlock[] Anomaly { get; }
    }
}
