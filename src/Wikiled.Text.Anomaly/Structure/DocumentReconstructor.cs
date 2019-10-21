using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class DocumentReconstructor : IDocumentReconstructor
    {
        public Document Reconstruct(ICollection<SentenceItem> sentences)
        {
            if (sentences == null)
            {
                throw new ArgumentNullException(nameof(sentences));
            }

            var cloned = sentences.CloneJson();
            var ordered = cloned.OrderBy(item => item.Index);
            var document = new Document();
            foreach (var sentenceItem in ordered)
            {
                document.Add(sentenceItem);
            }

            return document;
        }
    }
}
