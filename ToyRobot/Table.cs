namespace ToyRobot
{
    public class Table
    {
        private int _width;
        private int _height;

        public Table(int width = 5, int height = 5)
        {
            _width = width;
            _height = height;
        }

        public bool IsValidPosition(int x, int y) =>
            x >= 0 && x < _width && y >= 0 && y < _height;
    }
}
