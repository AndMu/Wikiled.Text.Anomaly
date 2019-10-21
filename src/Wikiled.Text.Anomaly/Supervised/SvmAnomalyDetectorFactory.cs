using System;
using System.IO;
using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.Text.Anomaly.Structure;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class SvmAnomalyDetectorFactory :  IModelFactory<SvmAnomalyDetector>
    {
        private readonly ILogger<SvmAnomalyDetectorFactory> logger;

        private readonly IDocumentVectorSource vectorSource;

        private readonly ILoggerFactory loggerFactory;

        public SvmAnomalyDetectorFactory(ILoggerFactory loggerFactory, IDocumentVectorSource vectorSource)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            logger = loggerFactory.CreateLogger<SvmAnomalyDetectorFactory>();
        }

        public SvmAnomalyDetector CreateNew()
        {
            return new SvmAnomalyDetector(loggerFactory, vectorSource, null);
        }

        public SvmAnomalyDetector Load(string path)
        {
            logger.LogDebug("Loading <{0}> model", path);
            var filePath = Path.Combine(path, "model.dat");
            var model = Serializer.Load<SupportVectorMachine<Linear>>(filePath);
            return new SvmAnomalyDetector(loggerFactory, vectorSource, model);
        }

        public void Save(string path, SvmAnomalyDetector model)
        {
            logger.LogInformation("Saving model...");
            model.Model.Save(Path.Combine(path, "model.dat"));
        }
    }
}
