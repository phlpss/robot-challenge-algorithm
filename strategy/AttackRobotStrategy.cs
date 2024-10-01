using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallenge
{
    public class AttackRobotStrategy : IRobotActionStrategy
    {
        private const double AttackPercentageGain = 0.1;
        private const double AttackMoveCost = 30;
        public (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            var (robotToAttack, profit) = FindBestRobotToAttack(robots, movingRobot);
            return profit > 0 ? (profit, new MoveCommand { NewPosition = robotToAttack.Position }) : (0, null);
        }

        public Tuple<Robot.Common.Robot, int> FindBestRobotToAttack(IList<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            var bestAttack = robots
                .Where(robot => !robot.OwnerName.Equals(currentRobot.OwnerName))
                .Select(robot => new
                {
                    Robot = robot,
                    MoveCost = MovementUtil.EnergyToMove(currentRobot.Position, robot.Position),
                    Profit = (int)(robot.Energy * AttackPercentageGain - AttackMoveCost - 
                                   MovementUtil.EnergyToMove(currentRobot.Position, robot.Position))
                })
                .Where(r => r.MoveCost <= r.Robot.Energy && r.Profit > 0)
                .OrderByDescending(r => r.Profit)
                .Select(r => Tuple.Create(r.Robot, r.Profit))
                .FirstOrDefault();

            return bestAttack ?? new Tuple<Robot.Common.Robot, int>(null, 0);
        }
    }

}
