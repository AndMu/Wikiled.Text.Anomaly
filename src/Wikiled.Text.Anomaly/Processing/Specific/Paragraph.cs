using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing.Specific
{
    public class Paragraph : IProcessingTextBlock
    {
        public Paragraph(SentenceItem[] sentences)
        {
            Sentences = sentences ?? throw new ArgumentNullException(nameof(sentences));
        }

        public SentenceItem[] Sentences { get; }
    }
}
