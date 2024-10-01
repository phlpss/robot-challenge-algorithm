using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Robot.Common;

namespace FilipKateryna.RobotChallange
{
    public class FilipKaterynaAlgorithm : IRobotAlgorithm
    {
        private readonly IRobotManager _robotManager;
        private readonly IMovementManager _movementManager;
        private readonly IEnumerable<IRobotActionStrategy> _actionStrategies;

        private int robotCount = 10;
        public string Author => "Filip Kateryna";
        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            _robotManager = new RobotManager();
            _movementManager = new MovementManager();
            _actionStrategies = new List<IRobotActionStrategy>
            {
                new CollectEnergyStrategy(new ProfitCalculator()),
                new AttackRobotStrategy(new ProfitCalculator()),
                new MoveToStationStrategy(new ProfitCalculator())
            };

            Logger.OnLogRound += (sender, e) => Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var movingRobot = robots[robotToMoveIndex];

            if (Round > 50) return new CollectEnergyCommand();

            var createCommand = _robotManager.CreateRobotIfNeeded(movingRobot, ref robotCount);
            if (createCommand != null) return createCommand;

            var tasks = _actionStrategies.Select(strategy => Task.Run(() => strategy.Execute(movingRobot, robots, map))).ToArray();
            Task.WaitAll(tasks);

            var actionProfits = tasks.Select(t => t.Result).OrderByDescending(a => a.Profit).FirstOrDefault();

            if (actionProfits.Profit > 0) return actionProfits.Command;

            return _movementManager.MoveCloserToStation(movingRobot, map, robots);
        }
    }

}