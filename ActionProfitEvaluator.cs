using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
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
