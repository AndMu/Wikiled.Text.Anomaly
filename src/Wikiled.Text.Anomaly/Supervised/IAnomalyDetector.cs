using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Model;

namespace Wikiled.Text.Anomaly.Supervised
{
    public interface IAnomalyDetector : IModel
    {
        bool Predict(IProcessingTextBlock data);

        bool[] Predict(IProcessingTextBlock[] data);

        double Probability(IProcessingTextBlock data);
    }
}