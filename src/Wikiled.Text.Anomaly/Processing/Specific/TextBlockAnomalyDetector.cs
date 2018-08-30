using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.IO;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Anomaly.Processing.Vectors;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing.Specific
{
    public class TextBlockAnomalyDetector<T>
        where T : IProcessingTextBlock
    {
        private SupportVectorMachine<Linear> model;

        //private Standardizer standardizer;

        private readonly IDocumentVectorSource vectorSource;

        private ILogger logger;

        public TextBlockAnomalyDetector(IDocumentVectorSource vectorSource, ILoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            logger = factory.CreateLogger(GetType());
        }

        public bool Predict(T data)
        {
            logger.LogDebug("Predict");
            double[][] observations = vectorSource.GetVectors(new IProcessingTextBlock[] { data }, NormalizationType.None);
            return model.Decide(observations[0]);
        }

        public bool[] Predict(T[] data)
        {
            logger.LogDebug("Predict");
            double[][] observations = vectorSource.GetVectors(data.Cast<IProcessingTextBlock>().ToArray(), NormalizationType.None);
            return model.Decide(observations);
        }

        public double Probability(T data)
        {
            logger.LogDebug("Probability");
            double[][] observations = vectorSource.GetVectors(new IProcessingTextBlock[] { data }, NormalizationType.None);
            return model.Probability(observations[0]);
        }

        public async Task Train(DataSet<T> dataset, CancellationToken token)
        {
            logger.LogDebug("Train");
            IProcessingTextBlock[] data = dataset.Positive.Concat(dataset.Negative).Cast<IProcessingTextBlock>().ToArray();
            int[] yData = dataset.Positive.Select(item => 1).Concat(dataset.Negative.Select(item => -1)).ToArray();
            double[][] xData = vectorSource.GetVectors(data, NormalizationType.None);
            Array[] randomized = GlobalSettings.Random.Shuffle(yData, xData).ToArray();
            //standardizer = Standardizer.GetNumericStandardizer(data);
            GridSearch<SupportVectorMachine<Linear>, double[], int> gridsearch = new GridSearch<SupportVectorMachine<Linear>, double[], int>
            {
                ParameterRanges =
                    new GridSearchRangeCollection
                    {
                        new GridSearchRange("complexity", new[] { 0.001, 0.01, 0.1, 1, 10 }),
                    },
                Learner = p => new LinearDualCoordinateDescent { Complexity = p["complexity"], Loss = Loss.L2 },
                Loss = (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            };


            gridsearch.Token = token;
            GridSearchResult<SupportVectorMachine<Linear>, double[], int> result = await Task.Run(() => gridsearch.Learn(randomized[1].Cast<double[]>().ToArray(), randomized[0].Cast<int>().ToArray()), token).ConfigureAwait(false);
            model = result.BestModel;
        }

        public void Save(string path)
        {
            logger.LogInformation("Save {0}", path);
            if (model == null)
            {
                throw new InvalidOperationException("Model is not trained");
            }

            model.Save(path);
        }

        public void Load(string path)
        {
            logger.LogInformation("Loading {0}", path);
            model = Serializer.Load<SupportVectorMachine>(path);
        }
    }
}
