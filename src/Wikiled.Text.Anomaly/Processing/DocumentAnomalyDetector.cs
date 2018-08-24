using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.MachineLearning.Clustering;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentAnomalyDetector : IDocumentAnomalyDetector
    {
        private const int MovingAverage = 3;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IAnomalyFilterFactory factory;

        private readonly IDocumentReconstructor reconstructor;

        private readonly List<TextCluster> anomaly = new List<TextCluster>();

        public DocumentAnomalyDetector(Document document, IAnomalyFilterFactory factory, IDocumentReconstructor reconstructor, int windowSize = 3)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            WindowSize = windowSize;
        }

        public TextCluster[] Anomaly => anomaly.ToArray();

        public Document Document { get; }

        public int WindowSize { get; }

        public Document Detect(params FilterTypes[] types)
        {
            if (types.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(types));
            }

            log.Debug("Detect");
            anomaly.Clear();
            if (Document.Sentences.Count <= 3)
            {
                log.Debug("Detect - text too short");
                return Document;
            }

            SentenceItem[][] sentenceClusters = null;
            log.Info("Using sentence clustering");
            sentenceClusters = GetSentencesBlock().ToArray();

            Document document = Document;
            TextCluster[] textClusters = sentenceClusters.Select(item => new TextCluster(item)).ToArray();
            foreach (FilterTypes filterTypese in types)
            {
                Document current = document.CloneJson();
                DetectionResults result = factory.Create(filterTypese).Filter(new DocumentClusters(current, textClusters));
                anomaly.AddRange(result.Anomaly);
                SentenceItem[] sentences = result.Result.SelectMany(item => item.Block).Distinct().ToArray();
                textClusters = result.Result;
                document = reconstructor.Reconstruct(sentences);
            }

            return document;
        }

        private SentenceItem[] GetData(ClusterRegion region)
        {
            int start = region.StartIndex == 0
                            ? 0
                            : region.StartIndex + MovingAverage / 2;
            int end = region.EndIndex + MovingAverage / 2;
            end = end > Document.Sentences.Count - 1 ? Document.Sentences.Count - 1 : end;

            List<SentenceItem> items = new List<SentenceItem>();
            for (int i = start; i < end; i++)
            {
                items.Add(Document.Sentences[i]);
            }

            return items.ToArray();
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlock()
        {
            foreach (var next in Document.Sentences.Window(WindowSize))
            {
                yield return next.ToArray();
            }
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlockForRegions(ClusterRegion[] regions)
        {
            return regions.Select(GetData);
        }
    }
}
