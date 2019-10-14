using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Structure.Model;

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