namespace ToyRobot
{
    public enum CommandType
    {
        Place,
        Move,
        Left,
        Right,
        Report,
        Unknown,
    }

    public record ParsedCommand(CommandType Type, CommandOptions? Options = null);

    public record CommandOptions(int X, int Y, Direction Facing);
}
