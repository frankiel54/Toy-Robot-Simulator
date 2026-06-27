using System;
using System.Collections.Generic;
using System.Text;

namespace ToyRobot
{
    public class Table
    {
        private int Width;
        private int Height;

        public Table(int width = 5, int height = 5)
        {
            Width = width;
            Height = height;
        }

        public bool IsValidPosition(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
