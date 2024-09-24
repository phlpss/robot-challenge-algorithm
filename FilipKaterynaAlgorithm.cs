using System;
using System.Collections.Generic;
using System.Linq;
using Robot.Common;

namespace FilipKateryna.RobotChallange
{
    public class FilipKaterynaAlgorithm : IRobotAlgorithm
    {
        private const int MaxRobots = 100;
        private const int NewRobotEnergyThreshold = 300;
        private const int NewRobotEnergy = 50;

        private int robotCount = 10;
        public string Author => "Filip Kateryna";
        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            Logger.OnLogRound += (sender, e) => Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobots = Functions.GetMyRobots(robots);
            var movingRobot = robots[robotToMoveIndex];

            if (movingRobot.Energy >= NewRobotEnergyThreshold && robotCount < MaxRobots)
            {
                robotCount++;
                return new CreateNewRobotCommand { NewRobotEnergy = NewRobotEnergy };
            }

            var actionProfits = new Dictionary<string, (int Profit, RobotCommand Command)>
            {
                { "CollectEnergy", (Functions.EnergyToBeCollected(map, movingRobot.Position), new CollectEnergyCommand()) },
                { "AttackRobot", Functions.CalculateAttackProfit(robots, movingRobot) },
                { "MoveToNearestStation", Functions.CalculateStationMoveProfit(movingRobot, map, robots, nearest: true) },
                { "MoveToBestStation", Functions.CalculateStationMoveProfit(movingRobot, map, robots, nearest: false) }
            };

            var bestAction = actionProfits.Values.OrderByDescending(a => a.Profit).FirstOrDefault();

            if (bestAction.Profit > 0)
            {
                return bestAction.Command;
            }

            return Functions.MoveCloserToStation(movingRobot, map, robots);
        }
    }
}