namespace ToyRobot
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
        Unset
    }

    public static class DirectionExtensions
    {
        public static Direction TurnLeft(this Direction direction) => direction switch
        {
            Direction.North => Direction.West,
            Direction.West  => Direction.South,
            Direction.South => Direction.East,
            Direction.East  => Direction.North,
            _               => direction,
        };

        public static Direction TurnRight(this Direction direction) => direction switch
        {
            Direction.North => Direction.East,
            Direction.East  => Direction.South,
            Direction.South => Direction.West,
            Direction.West  => Direction.North,
            _               => direction,
        };

    }
}
