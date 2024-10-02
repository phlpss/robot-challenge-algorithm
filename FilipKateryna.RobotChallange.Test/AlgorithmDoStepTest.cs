using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.Test
{
    [TestClass]
    public class AlgorithmDoStepTest
    {
        private FilipKaterynaAlgorithm _algorithm;
        private IList<Robot.Common.Robot> _robots;
        private Map _map;

        [TestInitialize]
        public void Setup()
        {
            _algorithm = new FilipKaterynaAlgorithm();
            _robots = CreateTestRobots();
            _map = CreateTestMap();
        }

        [TestCleanup]
        public void Teardown()
        {
            _robots = null;
            _map = null;
            _algorithm = null;
        }

        [TestMethod]
        public void DoStep_ShouldCreateNewRobot_WhenConditionMet()
        {
            var robotIndex = 0;
            var command = _algorithm.DoStep(_robots, robotIndex, _map);
            Assert.IsInstanceOfType(command, typeof(CreateNewRobotCommand));
            Assert.AreEqual(50, ((CreateNewRobotCommand)command).NewRobotEnergy);
        }

        [TestMethod]
        public void DoStep_ShouldCollectEnergy_WhenRobotIsAtStation()
        {
            var robotIndex = 1;
            var command = _algorithm.DoStep(_robots, robotIndex, _map);
            Assert.IsInstanceOfType(command, typeof(CollectEnergyCommand));
        }

        [TestMethod]
        public void DoStep_ShouldMoveTowardsBestRobotToAttack_WhenAttackIsMostProfitable()
        {
            var robotIndex = 2;
            SetupRobotsForAttackScenario();
            var command = _algorithm.DoStep(_robots, robotIndex, _map);
            Assert.IsInstanceOfType(command, typeof(MoveCommand));
            Assert.AreEqual(_robots[1].Position, ((MoveCommand)command).NewPosition);
        }

        [TestMethod]
        public void DoStep_ShouldMoveToBestStation_WhenStationHasHighestEnergy()
        {
            SetupMapWithBestStation();
            var command = _algorithm.DoStep(_robots, 0, _map);
            Assert.IsInstanceOfType(command, typeof(MoveCommand));
            Assert.AreEqual(new Position(4, 4), ((MoveCommand)command).NewPosition);
        }

        [TestMethod]
        public void DoStep_ShouldMoveCloserToStation_WhenNoProfitableActionIsAvailable()
        {
            SetupMapToStepCloserWhenProfitIsLow();
            var command = _algorithm.DoStep(_robots, 0, _map);
            Assert.IsInstanceOfType(command, typeof(MoveCommand));
            Assert.AreEqual(new Position(2, 2), ((MoveCommand)command).NewPosition);
        }

        [TestMethod]
        public void DoStep_ShouldCollectEnergyOnFinalRound()
        {
            _algorithm.Round = 51;
            var robotIndex = 0;
            var command = _algorithm.DoStep(_robots, robotIndex, _map);
            Assert.IsInstanceOfType(command, typeof(CollectEnergyCommand));
        }

        private IList<Robot.Common.Robot> CreateTestRobots()
        {
            return new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot { Position = new Position(0, 0), Energy = 300, OwnerName = "Filip Kateryna" },
                new Robot.Common.Robot { Position = new Position(3, 2), Energy = 200, OwnerName = "Boem Victor" },
                new Robot.Common.Robot { Position = new Position(5, 2), Energy = 100, OwnerName = "Franchuk Ivan" },
                new Robot.Common.Robot { Position = new Position(7, 6), Energy = 20, OwnerName = "Yaroshovych Sofia" },
                new Robot.Common.Robot { Position = new Position(11, 3), Energy = 100, OwnerName = "Lukjanov Nazar" },
                new Robot.Common.Robot { Position = new Position(0, 8), Energy = 1, OwnerName = "Vashchuk Victor" }
            };
        }

        private Map CreateTestMap()
        {
            return new Map
            {
                Stations = new List<EnergyStation>
                {
                    new EnergyStation { Position = new Position(2, 3), Energy = 100 },
                    new EnergyStation { Position = new Position(11, 6), Energy = 200 },
                    new EnergyStation { Position = new Position(5, 3), Energy = 150 },
                    new EnergyStation { Position = new Position(4, 9), Energy = 150 }
                }
            };
        }

        private void SetupRobotsForAttackScenario()
        {
            _robots[1].Energy = 1000;
            _robots[2].Energy = 100;

            foreach (var station in _map.Stations)
            {
                station.Energy = 50;
            }
        }

        private void SetupMapWithNearestStation()
        {
            _robots.Clear();
            _robots.Add(new Robot.Common.Robot { Position = new Position(4, 2), Energy = 100, OwnerName = "Test Name" });

            _map.Stations.Clear();
            _map.Stations.Add(new EnergyStation { Position = new Position(5, 3), Energy = 150 });
            _map.Stations.Add(new EnergyStation { Position = new Position(7, 6), Energy = 200 });
            _map.Stations.Add(new EnergyStation { Position = new Position(11, 3), Energy = 100 });
        }

        private void SetupMapWithBestStation()
        {
            _robots.Clear();
            _robots.Add(new Robot.Common.Robot { Position = new Position(3, 2), Energy = 100, OwnerName = "Test Name" });

            _map.Stations.Clear();
            _map.Stations.Add(new EnergyStation { Position = new Position(5, 3), Energy = 20 });
            _map.Stations.Add(new EnergyStation { Position = new Position(6, 6), Energy = 500 });
            _map.Stations.Add(new EnergyStation { Position = new Position(9, 3), Energy = 50 });
        }

        private void SetupMapToStepCloserWhenProfitIsLow()
        {
            _robots.Clear();
            _robots.Add(new Robot.Common.Robot { Position = new Position(1, 1), Energy = 100, OwnerName = "Test Name" });

            _map.Stations.Clear();
            _map.Stations.Add(new EnergyStation { Position = new Position(4, 4), Energy = 0 });
            _map.Stations.Add(new EnergyStation { Position = new Position(5, 5), Energy = 0 });
            _map.Stations.Add(new EnergyStation { Position = new Position(4, 7), Energy = 0 });
        }
    }
}
