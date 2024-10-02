using System;
using System.Collections.Generic;
using System.Linq;
using Robot.Common;

namespace FilipKateryna.RobotChallenge
{
    public static class MovementUtil
    {
        public static int EnergyToMove(Position a, Position b) =>
            (int)(Math.Pow(b.X - a.X, 2.0) + Math.Pow(b.Y - a.Y, 2.0));

        public static int Distance(Position pos1, Position pos2) =>
            Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);


        public static bool CellIsFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return robots.Where(robot => robot != movingRobot)
                         .All(robot => robot.Position != cell);
        }

        public static bool StationIsFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return Enumerable.Range(station.Position.X - 2, 5)
                .SelectMany(x => Enumerable.Range(station.Position.Y - 2, 5), (x, y) => new Position(x, y))
                .All(pos => CellIsFree(pos, movingRobot, robots));
        }
    }
}