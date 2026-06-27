using System;
using System.Collections.Generic;
using System.Text;

namespace ToyRobot
{
    public class GameBoard
    {
        private int XBoundary { get; }
        private int YBoundary { get; }
        private Robot Robot { get; }
        private bool RobotPlaced { get; set; }

        public GameBoard(int x, int y, Robot robot) {
            XBoundary = x;
            YBoundary = y;
            Robot = robot;
            RobotPlaced = false;
        }

        public bool Place(int x, int y, Direction direction)
        {
            if (x > XBoundary || y > YBoundary) {
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

            if (x < 0 || x > XBoundary || y < 0 || y > YBoundary) {
                return false;
            }

            Robot.xPos = x;
            Robot.yPos = y;

            return true;
        }


        public string Report()
        {
            return $"{Robot.xPos}, {Robot.yPos}, {Robot.direction.ToString()}";
        }

        public bool IsRobotPlaced () { return RobotPlaced; }
    }
}
