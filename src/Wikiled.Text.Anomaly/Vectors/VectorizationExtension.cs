using System;
using System.Threading.Tasks;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Vectors
{
    public static class VectorizationExtension
    {
        public static double[][] GetVectors(this IDocumentVectorSource source, IProcessingTextBlock[] blocks, NormalizationType normalization)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (blocks == null) throw new ArgumentNullException(nameof(blocks));

            double[][] observations = new double[blocks.Length][];

            Parallel.For(0,
                blocks.Length,
                i =>
                {
                    var result = source.GetVector(blocks[i], normalization).FullValues;
                    observations[i] = result;
                });

            return observations;
        }
    }
}
