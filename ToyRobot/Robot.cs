namespace ToyRobot
{
    public class Robot
    {
        public int XPos { get; private set; }
        public int YPos { get; private set; }
        public Direction Direction { get; private set; }

        public Robot()
        {
            XPos = -1;
            YPos = -1;
            Direction = Direction.Unset;
        }

        public Robot(int xPos, int yPos, Direction direction)
        {
            XPos = xPos;
            YPos = yPos;
            Direction = direction;
        }

        public void PlaceAt(int x, int y, Direction direction)
        {
            XPos = x;
            YPos = y;
            Direction = direction;
        }

        public void MoveTo(int x, int y)
        {
            XPos = x;
            YPos = y;
        }

        public void TurnLeft()  => Direction = Direction.TurnLeft();
        public void TurnRight() => Direction = Direction.TurnRight();

        public (int x, int y) GetNextPosition() => Direction switch
        {
            Direction.North => (XPos,     YPos + 1),
            Direction.South => (XPos,     YPos - 1),
            Direction.East  => (XPos + 1, YPos    ),
            Direction.West  => (XPos - 1, YPos    ),
            _               => (XPos,     YPos    ),
        };
    }
}
