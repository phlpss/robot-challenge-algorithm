using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.Test
{
    [TestClass]
    public class PositionManagerTest
    {
        private PositionManager _positionManager;

        [TestInitialize]
        public void Setup()
        {
            _positionManager = new PositionManager();
        }

        [TestMethod]
        public void MoveCloserToStation_ShouldReturnMoveCommandWhenStationIsFound()
        {
            var robot = new Robot.Common.Robot { Position = new Position(0, 0) };
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map
            {
                Stations = new List<EnergyStation> { new EnergyStation { Position = new Position(5, 5) } }
            };

            var command = _positionManager.MoveCloserToStation(robot, map, robots);

            Assert.IsInstanceOfType(command, typeof(MoveCommand));
            Assert.AreEqual(new Position(1, 1), ((MoveCommand)command).NewPosition);
        }

        [TestMethod]
        public void MoveCloserToStation_ShouldReturnCurrentPosition_WhenNoStationIsFound()
        {
            var robot = new Robot.Common.Robot { Position = new Position(0, 0) };
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map();

            var command = _positionManager.MoveCloserToStation(robot, map, robots);

            Assert.AreEqual(robot.Position, ((MoveCommand)command).NewPosition);
        }
    }
}