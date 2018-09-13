using System.Threading;
using System.Threading.Tasks;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Supervised
{
    public interface IModelStorage
    {
        IAnomalyDetector Current { get; }

        void Reset();

        void Add(DataType type, params IProcessingTextBlock[] blocks);

        IAnomalyDetector Load(string path);

        void Save(string path);

        Task<SvmAnomalyDetector> Train(CancellationToken token);
    }
}