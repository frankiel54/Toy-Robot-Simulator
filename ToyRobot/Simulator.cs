namespace ToyRobot
{
    public class Simulator
    {
        private Robot Robot { get; } = new Robot();
        private bool RobotPlaced { get; set; }
        private Table Table { get; }

        public Simulator(int width = 5, int height = 5)
        {
            Table = new Table(width, height);
        }

        public bool Place(int x, int y, Direction direction)
        {
            if (!Table.IsValidPosition(x, y))
                return false;

            Robot.PlaceAt(x, y, direction);
            RobotPlaced = true;

            return true;
        }

        public void TurnLeft()  => Robot.TurnLeft();

        public void TurnRight() => Robot.TurnRight();

        public bool MoveForward()
        {
            var (x, y) = Robot.GetNextPosition();

            if (!Table.IsValidPosition(x, y)) return false;

            Robot.MoveTo(x, y);
            return true;
        }

        public string Report() => $"{Robot.XPos}, {Robot.YPos}, {Robot.Direction}";

        public bool IsRobotPlaced() => RobotPlaced;

        public int X => Robot.XPos;
        public int Y => Robot.YPos;
        public Direction Facing => Robot.Direction;
    }
}
