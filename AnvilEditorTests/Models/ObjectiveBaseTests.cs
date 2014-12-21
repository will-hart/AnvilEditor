
namespace AnvilEditor.Models.Tests
{
    using System.Windows;
    using AnvilEditor;
    using AnvilEditor.Models;
    using Moq;
    using NUnit.Framework;


    [TestFixture()]
    public class ObjectiveBaseTests
    {
        [Test()]
        public void MapToCanvasXTest()
        {
            AnvilEditor.MainWindow.ScreenXMax = 300;
            AnvilEditor.MainWindow.MapXMin = 0;
            AnvilEditor.MainWindow.MapXMax = 100;
            Assert.AreEqual(150.0, ObjectiveBase.MapToCanvasX(50.0));
        }

        [Test()]
        public void CanvasToMapXTest()
        {
            AnvilEditor.MainWindow.ScreenXMax = 300;
            AnvilEditor.MainWindow.MapXMin = 0;
            AnvilEditor.MainWindow.MapXMax = 100;
            Assert.AreEqual(50.0, ObjectiveBase.CanvasToMapX(150.0));
        }

        [Test()]
        public void MapToCanvasYTest()
        {
            AnvilEditor.MainWindow.ScreenYMax = 300;
            AnvilEditor.MainWindow.MapYMin = 0;
            AnvilEditor.MainWindow.MapYMax = 100;
            Assert.AreEqual(150.0, ObjectiveBase.MapToCanvasY(50.0));
        }

        [Test()]
        public void CanvasToMapYTest()
        {
            AnvilEditor.MainWindow.ScreenYMax = 300;
            AnvilEditor.MainWindow.MapYMin = 0;
            AnvilEditor.MainWindow.MapYMax = 100;
            Assert.AreEqual(50.0, ObjectiveBase.CanvasToMapY(150.0));
        }

        [Test()]
        public void CanvasToMapYShouldReverseAxisDirection()
        {
            AnvilEditor.MainWindow.ScreenYMax = 300;
            AnvilEditor.MainWindow.MapYMin = 0;
            AnvilEditor.MainWindow.MapYMax = 100;
            Assert.AreEqual(25.0, ObjectiveBase.CanvasToMapY(225.0));
        }

        [Test()]
        public void CreateMarkerTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsOccupiedShouldChangeWhenTroopsAreAdded()
        {
            var b = new ObjectiveBase(1, new Point(2.0, 3.0));
            Assert.IsFalse(b.IsOccupied);

            b.Air = 1;
            Assert.IsTrue(b.IsOccupied);
        }
    }
}
