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
        (int Profit, RobotCommand Command) CalculateAttackProfit(IList<Robot.Common.Robot> robots, Robot.Common.Robot movingRobot);
        (int Profit, RobotCommand Command) DetermineBestAction(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots,
                                                               Map map, IEnumerable<IRobotActionStrategy> actionStrategies);
    }
    public class ProfitCalculator : IProfitCalculator
    {

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
