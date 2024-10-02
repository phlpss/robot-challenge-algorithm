using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilipKateryna.RobotChallenge.strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.Test
{
    [TestClass]
    public class ActionProfitEvaluatorTest
    {
        private ActionProfitEvaluator _evaluator;
        private Mock<IRobotActionStrategy> _mockStrategy;

        [TestInitialize]
        public void Setup()
        {
            _evaluator = new ActionProfitEvaluator();
            _mockStrategy = new Mock<IRobotActionStrategy>();
        }

        [TestMethod]
        public void DetermineBestAction_ShouldReturnBestProfitCommand()
        {
            var robot = new Robot.Common.Robot();
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map();

            _mockStrategy.Setup(s => s.Execute(robot, robots, map)).Returns((100, new CollectEnergyCommand()));
            var strategies = new List<IRobotActionStrategy> { _mockStrategy.Object };

            var result = _evaluator.DetermineBestAction(robot, robots, map, strategies);

            Assert.AreEqual(100 as object, result.Profit);
            Assert.IsInstanceOfType(result.Command, typeof(CollectEnergyCommand));
        }

        [TestMethod]
        public void DetermineBestAction_ShouldReturnHighestProfitActionFromMultipleStrategies()
        {
            var robot = new Robot.Common.Robot();
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map();

            var mockStrategy1 = new Mock<IRobotActionStrategy>();
            var mockStrategy2 = new Mock<IRobotActionStrategy>();

            mockStrategy1.Setup(s => s.Execute(robot, robots, map)).Returns((50, new CollectEnergyCommand()));
            mockStrategy2.Setup(s => s.Execute(robot, robots, map)).Returns((150, new CollectEnergyCommand()));

            var strategies = new List<IRobotActionStrategy> { mockStrategy1.Object, mockStrategy2.Object };

            var result = _evaluator.DetermineBestAction(robot, robots, map, strategies);

            Assert.AreEqual(150 as object, result.Profit);
            Assert.IsInstanceOfType(result.Command, typeof(CollectEnergyCommand));
        }

        [TestMethod]
        public void DetermineBestAction_ShouldReturnMoveCommandForAttackingRobot()
        {
            var robot = new Robot.Common.Robot { Position = new Position(5, 5) };
            var targetRobot = new Robot.Common.Robot { Position = new Position(6, 6) };
            var robots = new List<Robot.Common.Robot> { robot, targetRobot };
            var map = new Map();

            _mockStrategy.Setup(s => s.Execute(robot, robots, map))
                         .Returns((200, new MoveCommand { NewPosition = targetRobot.Position }));

            var strategies = new List<IRobotActionStrategy> { _mockStrategy.Object };

            var result = _evaluator.DetermineBestAction(robot, robots, map, strategies);

            Assert.AreEqual(200 as object, result.Profit);
            Assert.IsInstanceOfType(result.Command, typeof(MoveCommand));

            var attackCommand = (MoveCommand)result.Command;
            Assert.AreEqual(targetRobot.Position, attackCommand.NewPosition);
        }

        [TestMethod]
        public void DetermineBestAction_ShouldReturnMoveCommandToNearestStation()
        {
            var robot = new Robot.Common.Robot { Position = new Position(10, 10) };
            var station = new EnergyStation { Position = new Position(14, 14) };
            var map = new Map { Stations = new List<EnergyStation> { station } };
            var robots = new List<Robot.Common.Robot> { robot };

            var nearestCollectablePosition = new Position(12, 12);

            _mockStrategy.Setup(s => s.Execute(robot, robots, map))
                         .Returns((120, new MoveCommand { NewPosition = nearestCollectablePosition }));

            var strategies = new List<IRobotActionStrategy> { _mockStrategy.Object };

            var result = _evaluator.DetermineBestAction(robot, robots, map, strategies);

            Assert.AreEqual(120 as object, result.Profit);
            Assert.IsInstanceOfType(result.Command, typeof(MoveCommand));

            var moveCommand = (MoveCommand)result.Command;
            Assert.AreEqual(nearestCollectablePosition, moveCommand.NewPosition);
        }
    }
}