using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Robot.Common;

namespace FilipKateryna.RobotChallange
{
    public class FilipKaterynaAlgorithm : IRobotAlgorithm
    {
        private readonly RobotManager _robotManager;
        private readonly EnergyCalculator _energyCalculator;
        private readonly MovementManager _movementManager;
        private readonly Functions _functions;

        private int robotCount = 10;
        public string Author => "Filip Kateryna";
        public int Round { get; private set; }

        public FilipKaterynaAlgorithm()
        {
            _robotManager = new RobotManager();
            _energyCalculator = new EnergyCalculator();
            _movementManager = new MovementManager();
            _functions = new Functions();

            Logger.OnLogRound += (sender, e) => Round++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var movingRobot = robots[robotToMoveIndex];

            var createCommand = _robotManager.CreateRobotIfNeeded(movingRobot, ref robotCount);
            if (createCommand != null)
            {
                return createCommand;
            }

            var collectEnergyTask = Task.Run(() => _energyCalculator.CalculateEnergyCollectionProfit(map, movingRobot.Position));
            var attackRobotTask = Task.Run(() => _functions.CalculateAttackProfit(robots, movingRobot));
            var moveToStationTask = Task.Run(() => _functions.CalculateStationMoveProfit(movingRobot, map, robots));

            Task.WaitAll(collectEnergyTask, attackRobotTask, moveToStationTask);

            var actionProfits = new (int Profit, RobotCommand Command)[]
            {
            collectEnergyTask.Result,
            attackRobotTask.Result,
            moveToStationTask.Result
            };

            var bestAction = actionProfits.OrderByDescending(a => a.Profit).FirstOrDefault();

            if (bestAction.Profit > 0)
            {
                return bestAction.Command;
            }

            return _movementManager.MoveCloserToStation(movingRobot, map, robots, _functions);
        }
    }

}