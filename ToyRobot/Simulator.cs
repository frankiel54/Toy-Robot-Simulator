namespace ToyRobot
{
    public class Simulator
    {
        public const int DefaultWidth  = 5;
        public const int DefaultHeight = 5;

        private Robot? _robot;
        private Table Table { get; }

        public Simulator(int width = DefaultWidth, int height = DefaultHeight)
        {
            Table = new Table(width, height);
        }

        public bool Place(int x, int y, Direction direction)
        {
            if (!Table.IsValidPosition(x, y))
                return false;

            _robot = new Robot(x, y, direction);

            return true;
        }

        public void TurnLeft()
        {
            if (_robot is not null)
                _robot = _robot with { Direction = _robot.Direction.TurnLeft() };
        }

        public void TurnRight()
        {
            if (_robot is not null)
                _robot = _robot with { Direction = _robot.Direction.TurnRight() };
        }

        public bool MoveForward()
        {
            if (_robot is null) return false;

            var (x, y) = _robot.GetNextPosition();

            if (!Table.IsValidPosition(x, y)) return false;

            _robot = _robot with { XPos = x, YPos = y };
            return true;
        }

        public string Report()
        {
            if (_robot is null) throw new InvalidOperationException("Robot has not been placed.");
            return $"{_robot.XPos}, {_robot.YPos}, {_robot.Direction}";
        }

        public bool IsPlaced => _robot is not null;
    }
}
