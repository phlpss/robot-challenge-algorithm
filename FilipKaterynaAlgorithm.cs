using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Robot.Common;

namespace FilipKateryna.RobotChallange
{
    public class FilipKaterynaAlgorithm : IRobotAlgorithm
    {
        private const int maxRobots = 100;
        private int robotCount = 10;
        public string Author => "Filip Kateryna";

        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
        }

        private void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobots = Functions.GetMyRobots(robots);
            var movingRobot = robots[robotToMoveIndex];

            // 1. If there is enough energy and my robots count < 100, create new robot
            if (movingRobot.Energy >= 300 && robotCount < maxRobots)
            {
                robotCount++;
                return new CreateNewRobotCommand() { NewRobotEnergy = 50 };
            }

            // 2. Calculate profit of 4 possible moves:

            // 2.1) Collect energy on the current station
            var availableEnergy = Functions.EnergyToBeCollected(map, movingRobot.Position);

            // 2.2) Attack another robot
            var bestRobotToAttack = Functions.FindBestRobotToAttack(robots, movingRobot);
            var profitFromAttacking = bestRobotToAttack.Item2;

            // 2.3) Go to the nearest station
            var nearestStation = Functions.FindNearestFreeStation(movingRobot, map, robots);
            var profitFromNearestStation = nearestStation != null
                ? Functions.ProfitFromStationMove(movingRobot, nearestStation.Position, nearestStation.Energy)
                : 0;

            // 2.4) Go to the best station
            var bestStation = Functions.FindBestFreeStation(movingRobot, map, robots);
            var profitFromBestStation = bestStation != null
                ? Functions.ProfitFromStationMove(movingRobot, bestStation.Position, bestStation.Energy)
                : 0;

            // 3. Choose the move with the best profit
            var bestProfit = Math.Max(Math.Max(availableEnergy, profitFromAttacking), Math.Max(profitFromNearestStation, profitFromBestStation));

            // 4. If the profit is good, make the move
            if (bestProfit > 0)
            {
                if (bestProfit == availableEnergy)
                {
                    return new CollectEnergyCommand();
                }
                if (bestProfit == profitFromAttacking)
                {
                    return new MoveCommand() { NewPosition = bestRobotToAttack.Item1.Position };
                }
                if (bestProfit == profitFromNearestStation)
                {
                    Position nearestCollectablePosition = Functions.FindNearestCollectablePosition(movingRobot.Position, nearestStation.Position, 2);
                    return new MoveCommand() { NewPosition = nearestCollectablePosition };
                }
                if (bestProfit == profitFromBestStation)
                {
                    Position nearestCollectablePosition = Functions.FindNearestCollectablePosition(movingRobot.Position, bestStation.Position, 2);
                    return new MoveCommand() { NewPosition = nearestCollectablePosition };
                }
            }

            // 5. If profit is low, make 1 step further to the nearest/best station
            var stepCloserPosition = nearestStation != null
                ? Functions.GetOneStepCloser(movingRobot.Position, nearestStation.Position)
                : Functions.GetOneStepCloser(movingRobot.Position, bestStation.Position);

            return new MoveCommand() { NewPosition = stepCloserPosition };
        }
    }
}