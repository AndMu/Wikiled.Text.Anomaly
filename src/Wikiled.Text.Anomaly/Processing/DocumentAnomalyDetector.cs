using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Model;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentAnomalyDetector : IDocumentAnomalyDetector
    {
        private static readonly Logger<> log = LogManager.GetCurrentClassLogger();

        private readonly IAnomalyFilterFactory factory;

        private readonly IDocumentReconstructor reconstructor;

        private readonly List<IProcessingTextBlock> anomaly = new List<IProcessingTextBlock>();

        private readonly DocumentBlock document;

        public DocumentAnomalyDetector(DocumentBlock document, IAnomalyFilterFactory factory, IDocumentReconstructor reconstructor, int windowSize = 3)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            WindowSize = windowSize;
        }

        public int WindowSize { get; }

        public DetectionResult Detect(params FilterTypes[] types)
        {
            if(types.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(types));
            }

            log.Debug("Detect");
            anomaly.Clear();
            if(document.Sentences.Length <= 3)
            {
                log.Debug("Detect - text too short");
                return new DetectionResult(reconstructor.Reconstruct(document.Sentences), anomaly.ToArray());
            }

            log.Info("Using sentence clustering");
            var sentenceClusters = GetSentencesBlock().ToArray();

            foreach(FilterTypes filterTypes in types)
            {
                DetectionResults result = factory.Create(filterTypes).Filter(new ComplexDocument(sentenceClusters));
                anomaly.AddRange(result.Anomaly);
                sentenceClusters = result.Result;
            }

            return new DetectionResult(reconstructor.Reconstruct(sentenceClusters.SelectMany(item => item.Sentences).Distinct().ToArray()), anomaly.ToArray());
        }

        private IEnumerable<ProcessingTextBlock> GetSentencesBlock()
        {
            foreach(var next in document.Pages.Select(item => item.Sentences.Window(WindowSize)))
            {
                foreach(var block in next)
                {
                    yield return new ProcessingTextBlock(block.ToArray());
                }
            }
        }
    }
}
