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

        public void TurnLeft() {
            if (Robot.direction == Direction.North) {
                Robot.direction = Direction.West;
                return;
            }
            Robot.direction = Robot.direction - 1;
        }

        public void TurnRight()
        {
            if (Robot.direction == Direction.West)
            {
                Robot.direction = Direction.North;
                return;
            }
            Robot.direction = Robot.direction + 1;
        }

        public bool MoveForward()
        {
            var x = Robot.xPos;
            var y = Robot.yPos;

            switch (Robot.direction) {
                case Direction.North:
                    y = y + 1;
                    break;
                case Direction.South:
                    y = y - 1;
                    break;
                case Direction.East:
                    x = x + 1;
                    break;
                case Direction.West:
                    x = x - 1;
                    break;
            }

            if (!Table.IsValidPosition(x, y)) {
                return false;
            }

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
