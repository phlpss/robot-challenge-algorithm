using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public class MoveToStationStrategy : IRobotActionStrategy
    {
        private const int EnergyCollectableRadius = 2;

        public (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            var station = FindBestFreeStation(movingRobot, map, robots);
            if (station == null) return (0, null);

            var profit = ProfitFromStationMove(movingRobot, station.Position, station.Energy);
            var targetPosition = FindNearestCollectablePosition(movingRobot.Position, station.Position, EnergyCollectableRadius);

            return (profit, new MoveCommand { NewPosition = targetPosition });
        }

        public EnergyStation FindBestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            return map.Stations
                .Where(station => Functions.StationIsFree(station, movingRobot, robots)
                                && Functions.Distance(movingRobot.Position, station.Position) > 2
                                && Functions.Distance(movingRobot.Position, station.Position) < 23)
                .OrderByDescending(station => station.Energy)
                .ThenBy(station => Functions.EnergyToMove(station.Position, movingRobot.Position))
                .FirstOrDefault();
        }

        public int ProfitFromStationMove(Robot.Common.Robot movingRobot, Position stationPosition, int stationEnergy)
        {
            var nearestCollectablePosition = FindNearestCollectablePosition(movingRobot.Position, stationPosition, 2);
            return stationEnergy - Functions.EnergyToMove(movingRobot.Position, nearestCollectablePosition);
        }

        public Position FindNearestCollectablePosition(Position robotPosition, Position stationPosition, int radius)
        {
            return Enumerable.Range(stationPosition.X - radius, 2 * radius + 1)
                .SelectMany(x => Enumerable.Range(stationPosition.Y - radius, 2 * radius + 1), (x, y) => new Position(x, y))
                .OrderBy(pos => Functions.EnergyToMove(robotPosition, pos))
                .FirstOrDefault();
        }
    }
}
