using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallenge
{
    public class CollectEnergyStrategy : IRobotActionStrategy
    {
        private const int EnergyCollectableRadius = 2;
        public (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            var profit = CalculateEnergyToBeCollected(map, movingRobot.Position);
            return (profit, new CollectEnergyCommand());
        }

        private int CalculateEnergyToBeCollected(Map map, Position robotPos)
        {
            return map.Stations
                .Where(station =>
                    Math.Abs(station.Position.X - robotPos.X) <= EnergyCollectableRadius &&
                    Math.Abs(station.Position.Y - robotPos.Y) <= EnergyCollectableRadius)
                .Sum(station => station.Energy);
        }
    }
}