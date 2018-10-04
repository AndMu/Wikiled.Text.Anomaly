using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class SvmAnomalyDetector : IAnomalyDetector
    {
        private readonly IDocumentVectorSource vectorSource;

        private readonly ILogger logger;

        public SvmAnomalyDetector(IDocumentVectorSource vectorSource, ILoggerFactory factory, SupportVectorMachine<Linear> model)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            logger = factory.CreateLogger(GetType());
            Model = model;
        }

        public SupportVectorMachine<Linear> Model { get; private set; }

        public bool Predict(IProcessingTextBlock data)
        {
            logger.LogDebug("Predict");
            double[][] observations = vectorSource.GetVectors(new[] { data }, NormalizationType.None);
            return Model.Decide(observations[0]);
        }

        public bool[] Predict(IProcessingTextBlock[] data)
        {
            logger.LogDebug("Predict");
            double[][] observations = vectorSource.GetVectors(data, NormalizationType.None);
            return Model.Decide(observations);
        }

        public double Probability(IProcessingTextBlock data)
        {
            logger.LogDebug("Probability");
            double[][] observations = vectorSource.GetVectors(new[] { data }, NormalizationType.None);
            return Model.Probability(observations[0]);
        }

        public async Task Train(DataSet dataset, CancellationToken token)
        {
            logger.LogDebug("Train");
            IProcessingTextBlock[] data = dataset.Positive.Concat(dataset.Negative).ToArray();
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
            Model = result.BestModel;
        }
    }
}
