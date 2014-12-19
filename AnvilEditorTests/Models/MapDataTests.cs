namespace AnvilEditor.Models.Tests
{
    using Moq;
    using NUnit.Framework;
    using System.Linq;

    using AnvilEditor.Models;

    [TestFixture()]
    public class MapDataTests
    {
        [Test()]
        public void IsDownloadableFalseWhenNoString()
        {
            var f = new MapData();
            Assert.IsFalse(f.IsDownloadable);
        }

        [Test()]
        public void IsDownloadableTrueWhenHasStringAndNotDownloaded()
        {
            var mock = new Mock<MapData>();
            mock.SetupGet(o => o.DownloadUrl).Returns("asdf.com");

            var mapData = mock.Object;
            mapData.IsDownloaded = false;

            Assert.IsTrue(mapData.IsDownloadable);
        }

        [Test()]
        public void IsDownloadableFalseWhenHasStringAndDownloaded()
        {
            var mock = new Mock<MapData>();
            mock.SetupGet(o => o.DownloadUrl).Returns("asdf.com");

            var mapData = mock.Object;
            mapData.IsDownloaded = true;

            Assert.IsFalse(mapData.IsDownloadable);
        }
    }
}
