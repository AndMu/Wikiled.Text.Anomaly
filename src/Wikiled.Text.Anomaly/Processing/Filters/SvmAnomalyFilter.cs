using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using NLog;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class SvmAnomalyFilter : IAnomalyFilter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentVectorSource vectorSource;

        public SvmAnomalyFilter(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
        }

        public FilterTypes Type => FilterTypes.Svm;

        public DetectionResults Filter(ComplexDocument document)
        {
            if (document.Clusters.Length < 3)
            {
                logger.Info("Not enought text clusters for clustering");
                return new DetectionResults(document.Clusters);
            }

            double[][] observations = vectorSource.GetVectors(document.Clusters, NormalizationType.None);
            var standardizer = Standardizer.GetNumericStandardizer(observations);
            observations = standardizer.StandardizeAll(observations);
            var data = observations.ToArray();
            for (int i = 0; i < observations.Length; i++)
            {
                for (int j = 0; j < observations[i].Length; j++)
                {
                    if (double.IsNaN(observations[i][j]))
                    {
                        observations[i][j] = 0;
                    }
                }
            }

            var teacher = new OneclassSupportVectorLearning<Gaussian>
            {
                Kernel = Gaussian.FromGamma(1.0 / data.Length),
                Nu = 0.5,
                Shrinking = true,
                Tolerance = 0.001
            };

            var svm = teacher.Learn(data);
            double[] prediction = svm.Score(data);

            Dictionary<int, List<double>> weights = new Dictionary<int, List<double>>();
            for (int i = 0; i < prediction.Length; i++)
            {
                foreach (var sentenceItem in document.Clusters[i].Sentences)
                {
                    if (!weights.TryGetValue(sentenceItem.Index, out var classType))
                    {
                        classType = new List<double>();
                        weights[sentenceItem.Index] = classType;
                    }

                    classType.Add(prediction[i]);
                }
            }

            List<ProcessingTextBlock> anomaly = new List<ProcessingTextBlock>();
            List<ProcessingTextBlock> resultData = new List<ProcessingTextBlock>();
            List<SentenceItem> sentences = new List<SentenceItem>();
            ProcessingTextBlock cluster;
            bool? lastResult = null;
            var cutoffIndex = (int)(weights.Count * 0.2);
            var cutoff = weights.Select(item => item.Value.Sum()).OrderBy(item => item).Skip(cutoffIndex).First();
            var allSentences = document.Clusters.SelectMany(item => item.Sentences)
                .Distinct()
                .OrderBy(item => item.Index)
                .ToArray();
            if (allSentences.Length != weights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(document), "Sentence length mismatch");
            }

            foreach (var sentence in allSentences)
            {
                var current = weights[sentence.Index].Sum();
                var result = current > cutoff;
                if (lastResult != null &&
                    result != lastResult)
                {
                    cluster = new ProcessingTextBlock(sentences.ToArray());
                    sentences.Clear();
                    if (lastResult.Value)
                    {
                        resultData.Add(cluster);
                    }
                    else
                    {
                        anomaly.Add(cluster);
                    }
                }

                sentences.Add(sentence);
                lastResult = result;
            }

            cluster = new ProcessingTextBlock(sentences.ToArray());
            sentences.Clear();
            if (lastResult.Value)
            {
                resultData.Add(cluster);
            }
            else
            {
                anomaly.Add(cluster);
            }

            StringBuilder builder = new StringBuilder();
            foreach (var textCluster in anomaly)
            {
                foreach (var sentenceItem in textCluster.Sentences)
                {
                    builder.AppendLine(sentenceItem.Text);
                }
            }

            return new DetectionResults(resultData.ToArray(), anomaly.ToArray());
        }
    }
}
