using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Robot.Common;

namespace FilipKateryna.RobotChallange
{
    public class FilipKaterynaAlgorithm : IRobotAlgorithm
    {
        private readonly IRobotCreationManager _robotCreationManager;
        private readonly IPositionManager _positionManager;
        private readonly IActionProfitEvaluator _actionProfitEvaluator;
        private readonly IEnumerable<IRobotActionStrategy> _actionStrategies;

        private int robotCount = 10;
        public string Author => "Filip Kateryna";
        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            _robotCreationManager = new RobotCreationManager();
            _positionManager = new PositionManager();
            _actionProfitEvaluator = new ActionProfitEvaluator();

            _actionStrategies = new List<IRobotActionStrategy>
            {
                new CollectEnergyStrategy(),
                new AttackRobotStrategy(),
                new MoveToStationStrategy()
            };

            Logger.OnLogRound += (sender, e) => Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var movingRobot = robots[robotToMoveIndex];

            if (Round == 51)
                return new CollectEnergyCommand();

            var createCommand = _robotCreationManager.CreateRobotIfNeeded(movingRobot, ref robotCount);
            if (createCommand != null)
                return createCommand;

            var bestAction = _actionProfitEvaluator.DetermineBestAction(movingRobot, robots, map, _actionStrategies);

            if (bestAction.Profit > 0)
                return bestAction.Command;

            return _positionManager.MoveCloserToStation(movingRobot, map, robots);
        }
    }

}