using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class PageBlock : IProcessingTextBlock
    {
        public PageBlock(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Sentences = document.Sentences.ToArray();
        }

        public SentenceItem[] Sentences { get; }
    }
}
