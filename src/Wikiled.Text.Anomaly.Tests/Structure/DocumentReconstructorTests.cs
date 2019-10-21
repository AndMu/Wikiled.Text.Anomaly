using System;
using NUnit.Framework;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Structure;

namespace Wikiled.Text.Anomaly.Tests.Structure
{
    [TestFixture]
    public class DocumentReconstructorTests
    {
        private DocumentReconstructor instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public void Reconstruct()
        {
            SentenceItem[] items =
            {
                new SentenceItem("One")
                {
                    Index = 12
                },
                new SentenceItem("Three")
                {
                    Index = 2
                }
            };

            var result = instance.Reconstruct(items);
            Assert.AreEqual(2, result.Sentences.Count);
            Assert.AreEqual("Three", result.Sentences[0].Text);
            Assert.AreEqual(0, result.Sentences[0].Index);
            Assert.AreEqual("One", result.Sentences[1].Text);
            Assert.AreEqual(1, result.Sentences[1].Index);
        }

        [Test]
        public void Arguments()
        {
            Assert.Throws<ArgumentNullException>(() => instance.Reconstruct(null));
        }

        private DocumentReconstructor CreateInstance()
        {
            return new DocumentReconstructor();
        }
    }
}
