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

        public TextBlockAnomalyDetector(IDocumentVectorSource vectorSource)
        {
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
        }

        public bool Test(T data)
        {
            double[][] observations = vectorSource.GetVectors(new IProcessingTextBlock[] {data}, NormalizationType.None);
            return model.Decide(observations).First();
        }

        public async Task Train(DataSet<T> dataset, CancellationToken token)
        {
            var data = dataset.Positive.Concat(dataset.Negative).Cast<IProcessingTextBlock>().ToArray();
            var y = dataset.Positive.Select(item => 1).Concat(dataset.Negative.Select(item => -1)).ToArray();
            double[][] observations = vectorSource.GetVectors(data, NormalizationType.None);
            //standardizer = Standardizer.GetNumericStandardizer(data);
            var gridsearch = new GridSearch<SupportVectorMachine<Linear>, double[], int>
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
            var result = await Task.Run(() => gridsearch.Learn(observations, y), token);
            model = result.BestModel;
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }
    }
}
