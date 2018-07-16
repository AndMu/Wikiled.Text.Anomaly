using System;
using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public class TextCluster
    {
        private readonly ILookup<SentenceItem, SentenceItem> lookup;

        public TextCluster(params SentenceItem[] block)
        {
            Block = block ?? throw new ArgumentNullException(nameof(block));
            lookup = block.ToLookup(item => item);
        }

        public SentenceItem[] Block { get; }

        public bool Contains(SentenceItem sentence)
        {
            return lookup.Contains(sentence);
        }
    }
}
