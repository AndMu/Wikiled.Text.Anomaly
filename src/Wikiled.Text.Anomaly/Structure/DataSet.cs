using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class DataSet
    {
        public IProcessingTextBlock[] Positive { get; set; }

        public IProcessingTextBlock[] Negative { get; set; }
    }
}
