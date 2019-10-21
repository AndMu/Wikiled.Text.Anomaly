using System.Collections.Generic;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public interface IDocumentReconstructor
    {
        Document Reconstruct(ICollection<SentenceItem> sentences);
    }
}
