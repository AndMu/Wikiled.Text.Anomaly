using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;

namespace Wikiled.Text.Anomaly.Processing
{
    public static class DataTreeExtension
    {
        public static VectorData CreateVector(this IDataTree tree, NormalizationType normalization)
        {
            if (tree is null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            List<SimpleCell> vectors = new List<SimpleCell>();
            CreateVector("Data", tree, vectors);
            vectors = vectors.OrderBy(item => item.Name).ToList();
            return new VectorDataFactory().CreateSimple(normalization, vectors.Select(item => (ICell)item).ToArray());
        }

        private static void CreateVector(string prefix, IDataTree tree, List<SimpleCell> vector)
        {
            foreach (var leaf in tree.Leafs)
            {
                vector.Add(new SimpleCell(prefix + leaf.Name.CreatePureLetterText(), (double)leaf.Value));
            }

            string treeName = tree.Name.CreatePureLetterText();
            foreach (var branch in tree.Branches)
            {
                CreateVector(prefix + "_" + treeName + "_", branch, vector);
            }
        }
    }
}
