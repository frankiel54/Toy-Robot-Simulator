using System;
using System.Collections.Generic;
using System.Text;

namespace ToyRobot
{
    public class Simulator
    {
        private Robot Robot { get; }
        private bool RobotPlaced { get; set; }
        private Table Table { get; set; }

        public Simulator(Robot robot, int x = 5, int y = 5 ) {
            Robot = robot;
            RobotPlaced = false;
            Table = new Table(x, y);
        }

        public bool Place(int x, int y, Direction direction)
        {
            if (!Table.IsValidPosition(x,y)) {
                return false;
            }

            Robot.yPos = y;
            Robot.xPos = x;
            Robot.direction = direction;
            RobotPlaced = true;

            return true;
        }

        public void TurnLeft() => Robot.direction = Robot.direction.TurnLeft();

        public void TurnRight() => Robot.direction = Robot.direction.TurnRight();

        public bool MoveForward()
        {
            var (x, y) = Robot.GetNextPosition();

            if (!Table.IsValidPosition(x, y)) return false;

            Robot.xPos = x;
            Robot.yPos = y;
            return true;
        }


        public string Report()
        {
            return $"{Robot.xPos}, {Robot.yPos}, {Robot.direction}";
        }

        public bool IsRobotPlaced () { return RobotPlaced; }
    }
}
