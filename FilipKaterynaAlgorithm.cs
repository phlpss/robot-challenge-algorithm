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
        private readonly IProfitCalculator _profitCalculator;
        private readonly IEnumerable<IRobotActionStrategy> _actionStrategies;

        private int robotCount = 10;
        public string Author => "Filip Kateryna";
        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            _robotManager = new RobotManager();
            _movementManager = new MovementManager();
            _profitCalculator = new ProfitCalculator();

            _actionStrategies = new List<IRobotActionStrategy>
            {
                new CollectEnergyStrategy(),
                new AttackRobotStrategy(_profitCalculator),
                new MoveToStationStrategy(_profitCalculator)
            };

            Logger.OnLogRound += (sender, e) => Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var movingRobot = robots[robotToMoveIndex];

            if (Round == 51)
                return new CollectEnergyCommand();

            var createCommand = _robotManager.CreateRobotIfNeeded(movingRobot, ref robotCount);
            if (createCommand != null)
                return createCommand;

            var bestAction = _profitCalculator.DetermineBestAction(movingRobot, robots, map, _actionStrategies);

            if (bestAction.Profit > 0)
                return bestAction.Command;

            return _movementManager.MoveCloserToStation(movingRobot, map, robots);
        }
    }

}