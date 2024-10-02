using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilipKateryna.RobotChallenge.strategy;
using Robot.Common;

namespace FilipKateryna.RobotChallenge
{
    public interface IActionProfitEvaluator
    {
        (int Profit, RobotCommand Command) DetermineBestAction(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots,
                                                               Map map, IEnumerable<IRobotActionStrategy> actionStrategies);
    }
    public class ActionProfitEvaluator : IActionProfitEvaluator
    {
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
