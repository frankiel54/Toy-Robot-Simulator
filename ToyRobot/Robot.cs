using System;
using System.Collections.Generic;
using System.Text;

namespace ToyRobot
{
    public class Robot
    {
        public int xPos { get; set; } 
        public int yPos { get; set; }
        public Direction direction { get; set; }

        public Robot()
        {
            xPos = -1;
            yPos = -1;
            direction = Direction.Unset;
        }

        public (int x, int y) GetNextPosition() => direction switch
        {
            Direction.North => (xPos,     yPos + 1),
            Direction.South => (xPos,     yPos - 1),
            Direction.East  => (xPos + 1, yPos    ),
            Direction.West  => (xPos - 1, yPos    ),
            _               => (xPos,     yPos    ),
        };

    }
}
