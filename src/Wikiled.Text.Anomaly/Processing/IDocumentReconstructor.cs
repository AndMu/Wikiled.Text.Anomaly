using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public interface IDocumentReconstructor
    {
        Document Reconstruct(SentenceItem[] sentences);
    }
}