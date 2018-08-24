using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using NLog;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Anomaly.Processing.Vectors;

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

        public DetectionResults Filter(DocumentClusters document)
        {
            if (document.Clusters.Length < 3)
            {
                logger.Info("Not enought text clusters for clustering");
                return new DetectionResults(document.Clusters);
            }

            double[][] observations = new double[document.Clusters.Length][];

            Parallel.For(0,
                         document.Clusters.Length,
                         i =>
                         {
                             var documentCluster = document.Clusters[i];
                             var result = vectorSource.GetVector(new Paragraph(documentCluster.Block), NormalizationType.None).FullValues;
                             observations[i] = result;
                         });

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
                foreach (var sentenceItem in document.Clusters[i].Block)
                {
                    if (!weights.TryGetValue(sentenceItem.Index, out var classType))
                    {
                        classType = new List<double>();
                        weights[sentenceItem.Index] = classType;
                    }

                    classType.Add(prediction[i]);
                }
            }

            List<TextCluster> anomaly = new List<TextCluster>();
            List<TextCluster> resultData = new List<TextCluster>();
            List<SentenceItem> sentences = new List<SentenceItem>();
            TextCluster cluster;
            bool? lastResult = null;
            var cutoffIndex = (int)(weights.Count * 0.2);
            var cutoff = weights.Select(item => item.Value.Sum()).OrderBy(item => item).Skip(cutoffIndex).First();
            foreach (var sentence in document.Document.Sentences)
            {
                var current = weights[sentence.Index].Sum();
                var result = current > cutoff;
                if (lastResult != null &&
                    result != lastResult)
                {
                    cluster = new TextCluster(sentences.ToArray());
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


            cluster = new TextCluster(sentences.ToArray());
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
                foreach (var sentenceItem in textCluster.Block)
                {
                    builder.AppendLine(sentenceItem.Text);
                }
            }

            return new DetectionResults(resultData.ToArray(), anomaly.ToArray());
        }
    }
}
