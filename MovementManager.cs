using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public interface IMovementManager
    {
        RobotCommand MoveCloserToStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots);
    }

    public class MovementManager : IMovementManager
    {
        public RobotCommand MoveCloserToStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var station = FindNearestFreeStation(movingRobot, map, robots);
            if (station != null)
            {
                var stepCloserPosition = GetOneStepCloser(movingRobot.Position, station?.Position);
                if (stepCloserPosition != null && MovementUtil.CellIsFree(stepCloserPosition, movingRobot, robots))
                {
                    return new MoveCommand { NewPosition = stepCloserPosition };
                }
            }

            return new MoveCommand { NewPosition = movingRobot.Position };
        }

        private Position GetOneStepCloser(Position current, Position target)
        {
            int newX = current.X + (current.X < target.X ? 1 : current.X > target.X ? -1 : 0);
            int newY = current.Y + (current.Y < target.Y ? 1 : current.Y > target.Y ? -1 : 0);
            return new Position(newX, newY);
        }

        private EnergyStation FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            return map.Stations
                .Where(station => MovementUtil.StationIsFree(station, movingRobot, robots) 
                               && MovementUtil.Distance(movingRobot.Position, station.Position) > 2)
                .OrderBy(station => MovementUtil.EnergyToMove(station.Position, movingRobot.Position))
                .FirstOrDefault();
        }

    }
}
