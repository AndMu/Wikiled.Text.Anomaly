using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Clustering
{
    public class TextCluster
    {
        public TextCluster(SentenceItem[] block)
        {
            Block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public SentenceItem[] Block { get; }
    }
}
