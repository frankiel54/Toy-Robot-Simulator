namespace ToyRobot
{
    public record Robot(int XPos, int YPos, Direction Direction)
    {
        public (int x, int y) GetNextPosition() => Direction switch
        {
            Direction.North => (XPos,     YPos + 1),
            Direction.South => (XPos,     YPos - 1),
            Direction.East  => (XPos + 1, YPos    ),
            Direction.West  => (XPos - 1, YPos    ),
            _               => throw new ArgumentOutOfRangeException(nameof(Direction)),
        };
    }
}
