namespace ToyRobot
{
    public class Robot
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public Direction Direction { get; set; }

        public Robot()
        {
            XPos = -1;
            YPos = -1;
            Direction = Direction.Unset;
        }

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
