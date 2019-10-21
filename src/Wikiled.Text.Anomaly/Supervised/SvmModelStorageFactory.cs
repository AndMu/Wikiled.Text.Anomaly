using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class SvmModelStorageFactory : IModelStorageFactory<SvmAnomalyDetector>
    {
        private readonly ILoggerFactory loggerFactory;

        private readonly IDocumentReconstructor reconstructor;

        private readonly StorageConfig config;

        private readonly IModelFactory<SvmAnomalyDetector> modelFactory;

        public SvmModelStorageFactory(ILoggerFactory factory, IDocumentReconstructor reconstructor, StorageConfig config, IModelFactory<SvmAnomalyDetector> modelFactory)
        {
            this.loggerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.modelFactory = modelFactory;
            if (config.Location == null)
            {
                throw new ArgumentNullException(nameof(config.Location));
            }
        }

        public IModelStorage<SvmAnomalyDetector> Construct(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var location = Path.Combine(config.Location, name);
            var model = new ModelStorage<SvmAnomalyDetector>(loggerFactory.CreateLogger<ModelStorage<SvmAnomalyDetector>>(), reconstructor, modelFactory);
            if (Directory.Exists(location))
            {
                model.Load(location);
            }

            return model;
        }

        public void Save(string name, IModelStorage<SvmAnomalyDetector> storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            var location = Path.Combine(config.Location, name);
            storage.Save(location);
        }
    }
}
