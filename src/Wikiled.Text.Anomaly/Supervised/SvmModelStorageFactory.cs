﻿using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Supervised
{
    public class SvmModelStorageFactory : IModelStorageFactory
    {
        private readonly ILoggerFactory factory;

        private readonly IDocumentReconstructor reconstructor;

        private readonly IDocumentVectorSource vectorSource;

        private readonly LocationConfig config;

        public SvmModelStorageFactory(ILoggerFactory factory, IDocumentVectorSource vectorSource, IDocumentReconstructor reconstructor, LocationConfig config)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.vectorSource = vectorSource ?? throw new ArgumentNullException(nameof(vectorSource));
            this.reconstructor = reconstructor ?? throw new ArgumentNullException(nameof(reconstructor));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            if (config.Location == null)
            {
                throw new ArgumentNullException(nameof(config.Location));
            }
        }

        public IModelStorage Construct(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var location = Path.Combine(config.Location, name);
            var model = new SvmModelStorage(factory, vectorSource, reconstructor);
            if (Directory.Exists(location))
            {
                model.Load(location);
            }

            return model;
        }
    }
}
