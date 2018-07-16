using System;
using NUnit.Framework;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Anomaly.Processing;

namespace Wikiled.Text.Anomaly.Tests.Processing
{
    [TestFixture]
    public class DocumentAnomalyDetectorTests
    {
        private AnomalyFactory instance;

        private Document document;

        [SetUp]
        public void Setup()
        {
            document = Global.InitDocument("cv002_17424.txt");
            instance = new AnomalyFactory(Global.StyleFactory, Global.Dictionary);
        }

        [Test]
        public void Measure()
        {
            //var distances = instance.Create(document);
            //distances.Detect();
            //Assert.AreEqual(42, document.Sentences.Count);
            //Assert.AreEqual(32, distances.WithoutAnomaly.Length);
            //Assert.AreEqual(10, distances.Anomaly.Length);
            throw new NotImplementedException();
        }


        [Test]
        public void Anomaly()
        {
            //    var distances = instance.Create(document);
            //    distances.Detect();
            //    var anomaly = distances.Anomaly;
            //    Assert.AreEqual("Permission to make digital or hard copies of part or all of this work for personal or", distances.Anomaly[0].Text);
            //}
            throw new NotImplementedException();
        }

        [Test]
        public void MinimumSentences()
        {
            //var distances = instance.Create(document);
            //distances.Detect();
            //Assert.AreEqual(5, distances.MinimumSentencesCount);
            //distances.WindowSize = 0.01;
            //Assert.AreEqual(1, distances.MinimumSentencesCount);
            throw new NotImplementedException();
        }

        [Test]
        public void MinimumWords()
        {
            //var distances = instance.Create(document);
            //distances.Detect();
            //Assert.AreEqual(96, distances.MinimumWordsCount);
            //distances.WindowSize = 0.01;
            //Assert.AreEqual(10, distances.MinimumWordsCount);
            //distances.WindowSize = 0.0001;
            //Assert.AreEqual(1, distances.MinimumWordsCount);
            throw new NotImplementedException();
        }
    }
}
