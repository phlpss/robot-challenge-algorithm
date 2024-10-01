using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public class CollectEnergyStrategy : IRobotActionStrategy
    {
        private readonly IProfitCalculator _profitCalculator;

        public CollectEnergyStrategy(IProfitCalculator profitCalculator)
        {
            _profitCalculator = profitCalculator;
        }

        public (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            return _profitCalculator.CalculateEnergyCollectionProfit(map, movingRobot.Position);
        }
    }
}
