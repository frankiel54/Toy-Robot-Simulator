using System;
using System.Collections.Generic;
using System.Text;

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

    public class ParsedCommand {
        public CommandType Type { get; }
        public CommandOptions? Options { get; }

        public ParsedCommand(CommandType type, CommandOptions? options = null)
        {
            Type = type;
            Options = options;
        }
    }

    public class CommandOptions {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Facing { get; set; }
    }
}
