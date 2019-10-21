using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.Text.Anomaly.Structure
{
    public interface IModel
    {
        Task Train(DataSet dataSet, CancellationToken token);
    }
}
