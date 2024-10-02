using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.Test
{
    [TestClass]
    public class MovementUtilTests
    {
        [TestMethod]
        public void EnergyToMove_ShouldCalculateCorrectEnergy()
        {
            var positionA = new Position(0, 0);
            var positionB = new Position(3, 4);

            var energy = MovementUtil.EnergyToMove(positionA, positionB);

            Assert.AreEqual(25, energy);
        }

        [TestMethod]
        public void Distance_ShouldCalculateCorrectDistance()
        {
            var positionA = new Position(0, 0);
            var positionB = new Position(3, 4);

            var distance = MovementUtil.Distance(positionA, positionB);

            Assert.AreEqual(7, distance);
        }

        [TestMethod]
        public void CellIsFree_ShouldReturnTrue_WhenCellIsNotOccupied()
        {
            var robot1 = new Robot.Common.Robot { Position = new Position(1, 1) };
            var robot2 = new Robot.Common.Robot { Position = new Position(2, 2) };
            var robots = new List<Robot.Common.Robot> { robot1, robot2 };
            var movingRobot = robot1;
            var cell = new Position(3, 3);

            var isFree = MovementUtil.CellIsFree(cell, movingRobot, robots);

            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void CellIsFree_ShouldReturnFalse_WhenCellIsOccupied()
        {
            var robot1 = new Robot.Common.Robot { Position = new Position(1, 1) };
            var robot2 = new Robot.Common.Robot { Position = new Position(2, 2) };
            var robots = new List<Robot.Common.Robot> { robot1, robot2 };
            var movingRobot = robot1;
            var cell = new Position(2, 2);

            var isFree = MovementUtil.CellIsFree(cell, movingRobot, robots);

            Assert.IsFalse(isFree);
        }

        [TestMethod]
        public void StationIsFree_ShouldReturnTrue_WhenStationAreaIsFree()
        {
            var station = new EnergyStation { Position = new Position(5, 5), Energy = 100 };
            var robot1 = new Robot.Common.Robot { Position = new Position(1, 1) };
            var movingRobot = robot1;
            var robots = new List<Robot.Common.Robot> { robot1 };

            var isFree = MovementUtil.StationIsFree(station, movingRobot, robots);

            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void StationIsFree_ShouldReturnFalse_WhenStationAreaIsOccupied()
        {
            var station = new EnergyStation { Position = new Position(5, 5), Energy = 100 };
            var robot1 = new Robot.Common.Robot { Position = new Position(4, 5) };
            var robot2 = new Robot.Common.Robot { Position = new Position(1, 1) };
            var robots = new List<Robot.Common.Robot> { robot1, robot2 };
            var movingRobot = robot2;

            var isFree = MovementUtil.StationIsFree(station, movingRobot, robots);

            Assert.IsFalse(isFree);
        }
    }
}