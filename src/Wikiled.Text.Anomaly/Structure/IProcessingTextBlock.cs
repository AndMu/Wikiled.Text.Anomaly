using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public interface IProcessingTextBlock
    {
        SentenceItem[] Sentences { get; }
    }
}
