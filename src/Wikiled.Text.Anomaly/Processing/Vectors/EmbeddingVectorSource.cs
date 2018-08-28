using System.Linq;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Anomaly.Processing.Specific;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Processing.Vectors
{
    public class EmbeddingVectorSource : IDocumentVectorSource
    {
        private readonly WordModel model;

        private readonly VectorDataFactory vectorDataFactory = new VectorDataFactory();

        public EmbeddingVectorSource(WordModel model)
        {
            this.model = model;
        }

        public VectorData GetVector(IProcessingTextBlock textBlock, NormalizationType normalization)
        {
            var currentVector = model.GetParagraphVector(textBlock.Sentences);
            return vectorDataFactory.CreateSimple(normalization, Convert(currentVector));
        }

        private double[] Convert(float[] data)
        {
            double[] result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }

            return result;
        }
    }
}
