namespace ToyRobot
{
    public class Simulator
    {
        private Robot Robot { get; }
        private bool RobotPlaced { get; set; }
        private Table Table { get; set; }

        public Simulator(Robot robot, int width = 5, int height = 5)
        {
            Robot = robot;
            RobotPlaced = false;
            Table = new Table(width, height);
        }

        public bool Place(int x, int y, Direction direction)
        {
            if (!Table.IsValidPosition(x, y))
                return false;

            Robot.XPos = x;
            Robot.YPos = y;
            Robot.Direction = direction;
            RobotPlaced = true;

            return true;
        }

        public void TurnLeft() => Robot.Direction = Robot.Direction.TurnLeft();

        public void TurnRight() => Robot.Direction = Robot.Direction.TurnRight();

        public bool MoveForward()
        {
            var (x, y) = Robot.GetNextPosition();

            if (!Table.IsValidPosition(x, y)) return false;

            Robot.XPos = x;
            Robot.YPos = y;
            return true;
        }

        public string Report() => $"{Robot.XPos}, {Robot.YPos}, {Robot.Direction}";

        public bool IsRobotPlaced() => RobotPlaced;
    }
}
