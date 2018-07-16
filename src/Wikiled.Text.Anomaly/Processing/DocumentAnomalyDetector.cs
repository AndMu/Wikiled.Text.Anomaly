using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
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

        private readonly SentenceItem[] sentences;

        private IAnomalyFilterFactory factory;

        public DocumentAnomalyDetector(Document document, IAnomalyFilterFactory factory, double windowSize = 0.1)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            sentences = Document.Sentences.ToArray();
            WindowSize = windowSize;
        }

        public SentenceItem[] Anomaly { get; private set; }

        public Document Document { get; }

        public bool UseSentimentClusters { get; }

        public int MinimumSentencesCount => (int)Math.Ceiling(sentences.Length * WindowSize);

        public double MinimumWordsCount
        {
            get
            {
                return (int)Math.Ceiling(sentences.Sum(item => item.Words.Count) * WindowSize);
            }
        }

        public double WindowSize { get; }

        public Document Detect(params FilterTypes[] types)
        {
            throw new NotImplementedException();
            //log.Debug("Detect");
            //if (sentences.Length <= 3)
            //{
            //    log.Debug("Detect - text too short");

            //    return;
            //}

            //var ratings = sentences.Select(item => item.CalculateSentiment().RawRating)
            //                       .Select(item => item ?? 0)
            //                       .MovingAverage(3)
            //                       .ToArray();
            //ClusterRegion[] clusters = ClusterFlow.GetRegions(ratings, MovingAverage);

            //ConcurrentBag<IItemProbability<SentenceItem[]>> list = new ConcurrentBag<IItemProbability<SentenceItem[]>>();
            //IEnumerable<SentenceItem[]> sentenceClusters = UseSentimentClusters
            //                                                   ? GetSentencesBlockForRegions(clusters)
            //                                                   : GetSentencesBlock();
            //throw new NotImplementedException();

            //var processed = list.OrderBy(item => item.Probability);
            //Reset();

            //List<SentenceItem> excluding = new List<SentenceItem>();
            //foreach (var itemProbability in processed)
            //{
            //    if (excluding.Distinct().Count() >= AnomalySentencesCount)
            //    {
            //        break;
            //    }

            //    excluding.AddRange(itemProbability.Data);
            //}

            //if (excluding.Count > 0)
            //{
            //    Anomaly = excluding.ToArray();
            //    anomalyLookup = Anomaly.ToLookup(item => item);
            //    WithoutAnomaly = sentences.Where(item => !anomalyLookup.Contains(item)).ToArray();
            //}
        }

        private SentenceItem[] GetData(ClusterRegion region)
        {
            int start = region.StartIndex == 0
                            ? 0
                            : region.StartIndex + MovingAverage / 2;
            int end = region.EndIndex + MovingAverage / 2;
            end = end > sentences.Length - 1 ? sentences.Length - 1 : end;

            List<SentenceItem> items = new List<SentenceItem>();
            for (int i = start; i < end; i++)
            {
                items.Add(sentences[i]);
            }

            return items.ToArray();
        }
    
        private IEnumerable<SentenceItem[]> GetSentencesBlock()
        {
            return sentences.WindowedEx(
                MinimumSentencesCount,
                data => data.Select(item => item.Words.Count).Sum() >= MinimumWordsCount);
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlockForRegions(ClusterRegion[] regions)
        {
            return regions.Select(GetData);
        }
    }
}
