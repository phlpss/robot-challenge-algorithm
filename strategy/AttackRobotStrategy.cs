using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public class AttackRobotStrategy : IRobotActionStrategy
    {
        private readonly IProfitCalculator _profitCalculator;

        public AttackRobotStrategy(IProfitCalculator profitCalculator)
        {
            _profitCalculator = profitCalculator;
        }

        public (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            return _profitCalculator.CalculateAttackProfit(robots, movingRobot);
        }
    }

}
