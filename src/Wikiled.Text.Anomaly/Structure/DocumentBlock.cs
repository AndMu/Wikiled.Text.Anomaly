using System;
using System.Linq;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class DocumentBlock
    {
        public DocumentBlock(params Document[] document)
        {
            if(document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if(document.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(document));
            }

            Pages = document.Select(item => new PageBlock(item)).ToArray();
            Sentences = Pages.SelectMany(item => item.Sentences).ToArray();
            // reindex using global index
            int index = 0;
            foreach (var page in Pages)
            {
                foreach (var pageSentence in page.Sentences)
                {
                    pageSentence.Index = index;
                    index++;
                }
            }
        }

        public PageBlock[] Pages { get; }

        public SentenceItem[] Sentences { get; }
    }
}
