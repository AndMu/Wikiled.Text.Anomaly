using System;
using System.Threading.Tasks;

namespace Wikiled.Text.Anomaly.Processing.Specific
{
    public class TextBlockAnomalyDetector<T>
        where T : IProcessingTextBlock
    {
        public Task<bool> Test(T data)
        {
            throw new NotImplementedException();
        }

        public Task Train(DataSet<T> dataset)
        {
            throw new NotImplementedException();
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
