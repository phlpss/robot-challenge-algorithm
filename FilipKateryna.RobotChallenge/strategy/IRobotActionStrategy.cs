using System.Collections.Generic;
using Robot.Common;

namespace FilipKateryna.RobotChallenge.strategy
{
    public interface IRobotActionStrategy
    {
        (int Profit, RobotCommand Command) Execute(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map);
    }

}