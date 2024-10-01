using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public interface IProfitCalculator
    {
        (int Profit, RobotCommand Command) CalculateEnergyCollectionProfit(Map map, Position robotPosition);
        (int Profit, RobotCommand Command) CalculateAttackProfit(IList<Robot.Common.Robot> robots, Robot.Common.Robot movingRobot);
        (int Profit, RobotCommand Command) CalculateStationMoveProfit(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots);
        (int Profit, RobotCommand Command) DetermineBestAction(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots,
                                                               Map map, IEnumerable<IRobotActionStrategy> actionStrategies);
    }
    public class ProfitCalculator : IProfitCalculator
    {
        public (int Profit, RobotCommand Command) CalculateEnergyCollectionProfit(Map map, Position robotPosition)
        {
            var profit = Functions.CalculateEnergyToBeCollected(map, robotPosition);
            return (profit, new CollectEnergyCommand());
        }

        public (int Profit, RobotCommand Command) CalculateStationMoveProfit(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var station = Functions.FindBestFreeStation(movingRobot, map, robots);
            if (station == null) return (0, null);

            var profit = Functions.ProfitFromStationMove(movingRobot, station.Position, station.Energy);
            var targetPosition = Functions.FindNearestCollectablePosition(movingRobot.Position, station.Position, 2);

            return (profit, new MoveCommand { NewPosition = targetPosition });
        }

        public (int Profit, RobotCommand Command) CalculateAttackProfit(IList<Robot.Common.Robot> robots, Robot.Common.Robot movingRobot)
        {
            var (robotToAttack, profit) = Functions.FindBestRobotToAttack(robots, movingRobot);
            return profit > 0 ? (profit, new MoveCommand { NewPosition = robotToAttack.Position }) : (0, null);
        }

        public (int Profit, RobotCommand Command) DetermineBestAction(Robot.Common.Robot movingRobot,
            IList<Robot.Common.Robot> robots, Map map, IEnumerable<IRobotActionStrategy> actionStrategies)
        {
            var tasks = actionStrategies
                .Select(strategy => Task.Run(() => strategy.Execute(movingRobot, robots, map)))
                .ToArray();

            Task.WaitAll(tasks);

            return tasks.Select(t => t.Result)
                        .OrderByDescending(a => a.Profit)
                        .FirstOrDefault();
        }
    }
}
