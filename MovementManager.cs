using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallange
{
    public class MovementManager
    {
        public Position GetOneStepCloser(Position current, Position target)
        {
            int newX = current.X + (current.X < target.X ? 1 : current.X > target.X ? -1 : 0);
            int newY = current.Y + (current.Y < target.Y ? 1 : current.Y > target.Y ? -1 : 0);
            return new Position(newX, newY);
        }
        public RobotCommand MoveCloserToStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots, Functions functions)
        {
            var station = functions.FindNearestFreeStation(movingRobot, map, robots);
            if (station != null)
            {
                var stepCloserPosition = GetOneStepCloser(movingRobot.Position, station?.Position);
                if (stepCloserPosition != null && functions.CellIsFree(stepCloserPosition, movingRobot, robots))
                {
                    return new MoveCommand { NewPosition = stepCloserPosition };
                }
            }

            return new MoveCommand { NewPosition = movingRobot.Position };
        }
    }
}
