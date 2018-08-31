using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class DataSet
    {
        public IProcessingTextBlock[] Positive { get; set; }

        public IProcessingTextBlock[] Negative { get; set; }
    }
}
