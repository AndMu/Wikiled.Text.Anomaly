using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Specific
{
    public interface IProcessingTextBlock
    {
        SentenceItem[] Sentences { get; }
    }
}
