using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.Test
{
    [TestClass]
    public class RobotCreationManagerTest
    {
        private RobotCreationManager _robotCreationManager;

        [TestInitialize]
        public void Setup()
        {
            _robotCreationManager = new RobotCreationManager();
        }

        [TestMethod]
        public void CreateRobotIfNeeded_ShouldReturnCreateNewRobotCommand_WhenEnergyThresholdIsMetAndRobotCountIsLow()
        {
            var robot = new Robot.Common.Robot { Energy = 400 };
            int robotCount = 5;

            var command = _robotCreationManager.CreateRobotIfNeeded(robot, ref robotCount);

            Assert.IsInstanceOfType(command, typeof(CreateNewRobotCommand));
            Assert.AreEqual(6, robotCount);
        }

        [TestMethod]
        public void CreateRobotIfNeeded_ShouldReturnNull_WhenEnergyIsLow()
        {
            var robot = new Robot.Common.Robot { Energy = 200 };
            int robotCount = 5;

            var command = _robotCreationManager.CreateRobotIfNeeded(robot, ref robotCount);

            Assert.IsNull(command);
            Assert.AreEqual(5, robotCount); 
        }

        [TestMethod]
        public void CreateRobotIfNeeded_ShouldReturnNull_WhenMaxRobotCountIsReached()
        {
            var robot = new Robot.Common.Robot { Energy = 400 };
            int robotCount = 100;

            var command = _robotCreationManager.CreateRobotIfNeeded(robot, ref robotCount);

            Assert.IsNull(command);
            Assert.AreEqual(100, robotCount);
        }
    }
}
