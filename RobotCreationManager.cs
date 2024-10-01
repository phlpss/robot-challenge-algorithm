using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipKateryna.RobotChallenge
{
    public interface IRobotCreationManager
    {
        RobotCommand CreateRobotIfNeeded(Robot.Common.Robot movingRobot, ref int robotCount);
    }

    public class RobotCreationManager : IRobotCreationManager
    {
        private const int MaxRobots = 100;
        private const int NewRobotEnergyThreshold = 300;
        private const int NewRobotEnergy = 50;

        public RobotCommand CreateRobotIfNeeded(Robot.Common.Robot movingRobot, ref int robotCount)
        {
            if (movingRobot.Energy >= NewRobotEnergyThreshold && robotCount < MaxRobots)
            {
                robotCount++;
                return new CreateNewRobotCommand { NewRobotEnergy = NewRobotEnergy };
            }
            return null;
        }
    }
}
