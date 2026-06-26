namespace ToyRobot
{
    public static class CommandParser
    {
        public static bool TryParsePlaceArgs(string args, out int x, out int y, out Direction direction)
        {
            x = 0; y = 0; direction = default;
            var parts = args.Split(',');
            return parts.Length == 3
                && int.TryParse(parts[0].Trim(), out x)
                && int.TryParse(parts[1].Trim(), out y)
                && Enum.TryParse<Direction>(parts[2].Trim(), ignoreCase: true, out direction);
        }
    }
}
