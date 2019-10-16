using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Wikiled.Common.Testing.Utilities.Logging;
using Wikiled.Common.Testing.Utilities.Reflection;
using Wikiled.Text.Anomaly.Processing.Filters;
using Wikiled.Text.Anomaly.Vectors;

namespace Wikiled.Text.Anomaly.Tests.Processing.Filters
{
    [TestFixture]
    public class SvmAnomalyFilterTests
    {
        private Mock<IDocumentVectorSource> mockDocumentVectorSource;

        private SvmAnomalyFilter instance;

        private ILogger<SvmAnomalyFilter> logger = TestLogger.Create<SvmAnomalyFilter>();

        [SetUp]
        public void SetUp()
        {
            mockDocumentVectorSource = new Mock<IDocumentVectorSource>();
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            ConstructorHelper.ConstructorMustThrowArgumentNullException(typeof(SvmAnomalyFilter));
        }

        private SvmAnomalyFilter CreateInstance()
        {
            return new SvmAnomalyFilter(logger, mockDocumentVectorSource.Object);
        }
    }
}
