using System;
using Moq;
using NUnit.Framework;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Tests.Processing.Filters
{
    [TestFixture]
    public class SvmAnomalyFilterTests
    {
        private Mock<IDocumentVectorSource> mockDocumentVectorSource;

        private SvmAnomalyFilter instance;

        [SetUp]
        public void SetUp()
        {
            mockDocumentVectorSource = new Mock<IDocumentVectorSource>();
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SvmAnomalyFilter(null));
        }

        private SvmAnomalyFilter CreateInstance()
        {
            return new SvmAnomalyFilter(mockDocumentVectorSource.Object);
        }
    }
}
