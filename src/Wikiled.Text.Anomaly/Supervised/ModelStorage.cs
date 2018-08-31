using System;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class ModelStorage
    {
        public void Add(PositiveNegative type, params IProcessingTextBlock[] blocks)
        {
            throw new NotImplementedException();
        }

        public TextBlockAnomalyDetector Train()
        {
            throw new NotImplementedException();
        }

        public TextBlockAnomalyDetector Load(string path)
        {
            throw new NotImplementedException();
        }
    }
}
