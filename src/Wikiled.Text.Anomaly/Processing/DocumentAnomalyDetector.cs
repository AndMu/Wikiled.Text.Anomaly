using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.MachineLearning.Clustering;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentAnomalyDetector : IDocumentAnomalyDetector
    {
        private const int MovingAverage = 3;

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly SentenceItem[] sentences;

        private ILookup<SentenceItem, SentenceItem> anomalyLookup;

        private double anomalyThreshold;

        private double windowSize;

        public DocumentAnomalyDetector(Document document)
        {
            Document = document;
            sentences = Document.Sentences.ToArray();
            AnomalyThreshold = 0.1;
            windowSize = 0.1;
        }

        public SentenceItem[] Anomaly { get; private set; }

        public int AnomalySentencesCount => (int)Math.Ceiling(sentences.Length * AnomalyThreshold);

        public double AnomalyThreshold
        {
            get => anomalyThreshold;
            set
            {
                if (value <= 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("AnomalyThreshold");
                }

                anomalyThreshold = value;
            }
        }

        public Document Document { get; }

        public int MinimumSentencesCount => (int)Math.Ceiling(sentences.Length * WindowSize);

        public double MinimumWordsCount
        {
            get
            {
                return (int)Math.Ceiling(sentences.Sum(item => item.Words.Count) * WindowSize);
            }
        }

        public bool UseSentimentClusters { get; set; }

        public bool UseVector { get; set; }

        public double WindowSize
        {
            get => windowSize;
            set
            {
                if (value <= 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("WindowSize");
                }

                windowSize = value;
            }
        }

        public SentenceItem[] WithoutAnomaly { get; private set; }

        public void Detect()
        {
            log.Debug("Detect");
            if (sentences.Length <= 3)
            {
                log.Debug("Detect - text too short");
                return;
            }

            var ratings = sentences.Select(item => item.CalculateSentiment())
                                   .MovingAverage(3)
                                   .ToArray();
            ClusterRegion[] clusters = ClusterFlow.GetRegions(ratings, MovingAverage);

            ConcurrentBag<IItemProbability<SentenceItem[]>> list = new ConcurrentBag<IItemProbability<SentenceItem[]>>();
            IEnumerable<SentenceItem[]> sentenceClusters = UseSentimentClusters
                                                               ? GetSentencesBlockForRegions(clusters)
                                                               : GetSentencesBlock();
            throw new NotImplementedException();

            var processed = list.OrderBy(item => item.Probability);
            Reset();

            List<SentenceItem> excluding = new List<SentenceItem>();
            foreach (var itemProbability in processed)
            {
                if (excluding.Distinct().Count() >= AnomalySentencesCount)
                {
                    break;
                }

                excluding.AddRange(itemProbability.Data);
            }

            if (excluding.Count > 0)
            {
                Anomaly = excluding.ToArray();
                anomalyLookup = Anomaly.ToLookup(item => item);
                WithoutAnomaly = sentences.Where(item => !anomalyLookup.Contains(item)).ToArray();
            }
        }

        public SentenceItem[] GetData(ClusterRegion region)
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

        
        public bool IsInAnomaly(SentenceItem sentence)
        {
            return anomalyLookup != null && anomalyLookup.Contains(sentence);
        }

        public void Reset()
        {
            Anomaly = new SentenceItem[] { };
            WithoutAnomaly = sentences;
            anomalyLookup = null;
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
