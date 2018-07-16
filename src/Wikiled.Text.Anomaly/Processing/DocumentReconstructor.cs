using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.IO;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Text.Anomaly.Processing
{
    public class DocumentReconstructor
    {
        public Document Reconstruct(SentenceItem[] sentences)
        {
            var ordered = sentences.OrderBy(item => item.Index);
            sentences.DeepClone()
            Document document = new Document();
        }
    }
}
