using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Reflection.Data;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Style.Logic;

namespace Wikiled.Text.Anomaly.Processing.Vectors
{
    public class DocumentVectorSource : IDocumentVectorSource
    {
        private static readonly IMapCategory MapFull = new CategoriesMapper().Construct<TextBlock>();

        private readonly IStyleFactory styleFactory;

        private readonly INRCDictionary dictionary;

        private readonly AnomalyVectorType anomalyVectorType;

        public DocumentVectorSource(IStyleFactory styleFactory, INRCDictionary dictionary, AnomalyVectorType anomalyVectorType)
        {
            this.styleFactory = styleFactory ?? throw new ArgumentNullException(nameof(styleFactory));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.anomalyVectorType = anomalyVectorType;
        }

        public VectorData GetVector(IProcessingTextBlock normalBlock, NormalizationType normalization)
        {
            var normal = styleFactory.ConstructTextBlock(normalBlock.Sentences);
            DataTree tree;
            switch (anomalyVectorType)
            {
                case AnomalyVectorType.Full:
                    tree = new DataTree(normal, MapFull);
                    break;
                case AnomalyVectorType.Inquirer:
                    tree = normal.InquirerFinger.InquirerProbabilities;
                    break;
                case AnomalyVectorType.SentimentCategory:
                    tree = GetTree(normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("AnomalyVectorType");
            }

            VectorData vector = tree.CreateVector(normalization);
            return vector;
        }

        private DataTree GetTree(ITextBlock block)
        {
            var vector = dictionary.Extract(block.Words);
            return vector.GetTree();
        }
    }
}
