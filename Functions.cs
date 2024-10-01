using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace FilipKateryna.RobotChallange
{
    public class Functions
    {
        public int EnergyToMove(Position a, Position b) =>
            (int)(Math.Pow(b.X - a.X, 2.0) + Math.Pow(b.Y - a.Y, 2.0));

        public int Distance(Position pos1, Position pos2) =>
            Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);

        public bool StationIsFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return Enumerable.Range(station.Position.X - 2, 5)
                .SelectMany(x => Enumerable.Range(station.Position.Y - 2, 5), (x, y) => new Position(x, y))
                .All(pos => CellIsFree(pos, movingRobot, robots));
        }

        public bool CellIsFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return robots.Where(robot => robot != movingRobot)
                         .All(robot => robot.Position != cell);
        }

        public EnergyStation FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            return map.Stations
                .Where(station => StationIsFree(station, movingRobot, robots) && Distance(movingRobot.Position, station.Position) > 2)
                .OrderBy(station => EnergyToMove(station.Position, movingRobot.Position))
                .FirstOrDefault();
        }

        public EnergyStation FindBestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            return map.Stations
                .Where(station => StationIsFree(station, movingRobot, robots)
                                && Distance(movingRobot.Position, station.Position) > 2
                                && Distance(movingRobot.Position, station.Position) < 23)
                .OrderByDescending(station => station.Energy)
                .ThenBy(station => EnergyToMove(station.Position, movingRobot.Position))
                .FirstOrDefault();
        }

        public Tuple<Robot.Common.Robot, int> FindBestRobotToAttack(IList<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            var bestAttack = robots
                .Where(robot => robot != currentRobot && !robot.OwnerName.Equals(currentRobot.OwnerName))
                .Select(robot => new
                {
                    Robot = robot,
                    MoveCost = EnergyToMove(currentRobot.Position, robot.Position),
                    Profit = (int)(robot.Energy * 0.1 - 30 - EnergyToMove(currentRobot.Position, robot.Position))
                })
                .Where(r => r.MoveCost <= r.Robot.Energy && r.Profit > 0)
                .OrderByDescending(r => r.Profit)
                .Select(r => Tuple.Create(r.Robot, r.Profit))
                .FirstOrDefault();

            return bestAttack ?? new Tuple<Robot.Common.Robot, int>(null, 0);
        }

        public static List<Robot.Common.Robot> GetMyRobots(IList<Robot.Common.Robot> allRobots) =>
            allRobots.Where(robot => robot.OwnerName.Equals("Filip Kateryna")).ToList();


        public int ProfitFromStationMove(Robot.Common.Robot movingRobot, Position stationPosition, int stationEnergy)
        {
            var nearestCollectablePosition = FindNearestCollectablePosition(movingRobot.Position, stationPosition, 2);
            return stationEnergy - EnergyToMove(movingRobot.Position, nearestCollectablePosition);
        }

        public Position FindNearestCollectablePosition(Position robotPosition, Position stationPosition, int radius)
        {
            return Enumerable.Range(stationPosition.X - radius, 2 * radius + 1)
                .SelectMany(x => Enumerable.Range(stationPosition.Y - radius, 2 * radius + 1), (x, y) => new Position(x, y))
                .OrderBy(pos => EnergyToMove(robotPosition, pos))
                .FirstOrDefault();
        }

        public (int Profit, RobotCommand Command) CalculateAttackProfit(IList<Robot.Common.Robot> robots, Robot.Common.Robot movingRobot)
        {
            var (robotToAttack, profit) = FindBestRobotToAttack(robots, movingRobot);
            return profit > 0 ? (profit, new MoveCommand { NewPosition = robotToAttack.Position }) : (0, null);
        }

        public (int Profit, RobotCommand Command) CalculateStationMoveProfit(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var station = FindBestFreeStation(movingRobot, map, robots);
            if (station == null) return (0, null);

            var profit = ProfitFromStationMove(movingRobot, station.Position, station.Energy);
            var targetPosition = FindNearestCollectablePosition(movingRobot.Position, station.Position, 2);

            return (profit, new MoveCommand { NewPosition = targetPosition });
        }

    }
}