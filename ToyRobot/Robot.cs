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

        public Robot (){
            xPos = -1;
            yPos = -1;
            direction = Direction.Unset;
        }

    }
}
