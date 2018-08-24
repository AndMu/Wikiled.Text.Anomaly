using System;
using System.Linq;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentReconstructor : IDocumentReconstructor
    {
        public Document Reconstruct(SentenceItem[] sentences)
        {
            if (sentences == null)
            {
                throw new ArgumentNullException(nameof(sentences));
            }

            var cloned = sentences.CloneJson();
            var ordered = cloned.OrderBy(item => item.Index);
            Document document = new Document();
            foreach (var sentenceItem in ordered)
            {
                var original = sentenceItem.Index;
                document.Add(sentenceItem);
                sentenceItem.Index = original;
            }

            return document;
        }
    }
}
