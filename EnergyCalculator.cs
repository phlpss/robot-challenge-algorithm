using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public class EnergyCalculator
    {
        public int CalculateEnergyToBeCollected(Map map, Position robotPos)
        {
            return map.Stations
                .Where(station =>
                    Math.Abs(station.Position.X - robotPos.X) <= 2 &&
                    Math.Abs(station.Position.Y - robotPos.Y) <= 2)
                .Sum(station => station.Energy);
        }
        public (int Profit, RobotCommand Command) CalculateEnergyCollectionProfit(Map map, Position robotPosition)
        {
            var profit = CalculateEnergyToBeCollected(map, robotPosition);
            return (profit, new CollectEnergyCommand());
        }

    }
}
