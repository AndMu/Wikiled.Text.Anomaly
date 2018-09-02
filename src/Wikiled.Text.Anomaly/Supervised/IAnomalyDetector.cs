using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Supervised
{
    public interface IAnomalyDetector
    {
        bool Predict(IProcessingTextBlock data);

        bool[] Predict(IProcessingTextBlock[] data);

        double Probability(IProcessingTextBlock data);

        Task Train(DataSet dataset, CancellationToken token);
    }
}