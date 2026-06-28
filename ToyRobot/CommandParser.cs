namespace ToyRobot
{
    public static class CommandParser
    {
        public static ParsedCommand ParseCommand(string input)
        {
            input = input.Trim();
            var parts = input.Split(' ', 2);
            var name = parts[0].ToUpper();
            var rawArgs = parts.Length > 1 ? parts[1] : string.Empty;

            var type = name switch
            {
                "PLACE"  => CommandType.Place,
                "MOVE"   => CommandType.Move,
                "LEFT"   => CommandType.Left,
                "RIGHT"  => CommandType.Right,
                "REPORT" => CommandType.Report,
                _        => CommandType.Unknown,
            };

            if (type == CommandType.Place)
            {
                CommandOptions? options = TryParsePlaceArgs(rawArgs, out int x, out int y, out Direction facing)
                    ? new CommandOptions(x, y, facing)
                    : null;
                return new ParsedCommand(CommandType.Place, options);
            }

            return new ParsedCommand(type);
        }

        private static bool TryParsePlaceArgs(string args, out int x, out int y, out Direction direction)
        {
            x = 0; y = 0; direction = default;
            var parts = args.Split(',');
            if (parts.Length == 3
                && int.TryParse(parts[0].Trim(), out x)
                && int.TryParse(parts[1].Trim(), out y)
                && Enum.TryParse<Direction>(parts[2].Trim(), ignoreCase: true, out direction))
                return true;

            return false;
        }
    }
}
