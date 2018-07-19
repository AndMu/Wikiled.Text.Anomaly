using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.MachineLearning.Clustering;
using Wikiled.MachineLearning.Normalization;
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

        public DocumentAnomalyDetector(Document document,
                                       IAnomalyFilterFactory factory,
                                       IDocumentReconstructor reconstructor,
                                       bool useSentimentClusters = false,
                                       int minimumSentences = 3)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            UseSentimentClusters = useSentimentClusters;
            MinimumSentencesCount = minimumSentences;
        }

        public TextCluster[] Anomaly => anomaly.ToArray();

        public Document Document { get; }

        public bool UseSentimentClusters { get; }

        public int MinimumSentencesCount { get; }

        public Document Detect(params FilterTypes[] types)
        {
            if (types.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(types));
            }

            log.Debug("Detect");
            anomaly.Clear();
            if (Document.Sentences.Count <= 2 * MinimumSentencesCount)
            {
                log.Debug("Detect - text too short");
                return Document;
            }

            var ratings = Document.Sentences.Select(item => item.CalculateSentiment().RawRating)
                                   .Select(item => item ?? 0)
                                   .MovingAverage(3)
                                   .ToArray();
            
            SentenceItem[][] sentenceClusters = null;
            if (UseSentimentClusters)
            {
                ClusterRegion[] clusters = ClusterFlow.GetRegions(ratings, MovingAverage);
                if (clusters.Length == 0)
                {
                    log.Info("Failed to create sentiment clusters");
                }
                else
                {
                    sentenceClusters = GetSentencesBlockForRegions(clusters).ToArray();
                }
            }

            if (sentenceClusters == null)
            {
                log.Info("Using sentence clustering");
                sentenceClusters = GetSentencesBlock().ToArray();
            }
            
            var document = Document;
            var textClusters = sentenceClusters.Select(item => new TextCluster(item)).ToArray();
            foreach (var filterTypes in types)
            {
                var current = document.CloneJson();
                var result = factory.Create(filterTypes).Filter(new DocumentClusters(current, textClusters));
                anomaly.AddRange(result.Anomaly);
                var sentences = result.Result.SelectMany(item => item.Block).Distinct().ToArray();
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
            return Document.Sentences.Windowed(MinimumSentencesCount);
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlockForRegions(ClusterRegion[] regions)
        {
            return regions.Select(GetData);
        }
    }
}
