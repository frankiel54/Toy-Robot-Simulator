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

        public GameBoard(int x, int y, Robot robot) {
            XBoundary = x;
            YBoundary = y;
            Robot = robot;
        }


        public bool Place(int x, int y, Direction direction)
        {
            if (x > XBoundary || y > YBoundary) {
                return false;
            }

            Robot.yPos = y;
            Robot.xPos = x;
            Robot.direction = direction;

            return true;
        }

    }
}
