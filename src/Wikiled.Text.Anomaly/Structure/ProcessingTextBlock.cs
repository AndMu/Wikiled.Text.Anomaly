using System;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Structure
{
    public class ProcessingTextBlock : IProcessingTextBlock
    {
        public ProcessingTextBlock(params SentenceItem[] sentences)
        {
            Sentences = sentences ?? throw new ArgumentNullException(nameof(sentences));
        }

        public SentenceItem[] Sentences { get; }
    }
}
