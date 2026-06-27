using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Xunit;

namespace ToyRobot.Tests
{
    public class CommandParserTests
    {
        [Theory]
        [MemberData(nameof(ParseCommandData))]
        public void ParseCommand_Should_Parse(string rawCommand, string parsedCommand, string parsedArgs)
        {
            CommandParser.ParseCommand(rawCommand, out string command, out string commandArgs);

            Assert.Equal(parsedCommand, command);
            Assert.Equal(parsedArgs, commandArgs);
        }

        public static TheoryData<string, string, string> ParseCommandData() => new()
        {
            { "PLACE 1 1 NORTH","PLACE", "1 1 NORTH"},
            { "MOVE","MOVE", String.Empty},
            { "COMMAND1","COMMAND1", String.Empty},
        };

        [Theory]
        [MemberData(nameof(TryParsePlaceArgsData))]
        public void TryParsePlaceArgs_Should_Parse(string rawCommand, int expectedX, int expectedY, Direction expectedDirection)
        {
            CommandParser.TryParsePlaceArgs(rawCommand, out int x, out int y, out Direction direction);

            Assert.Equal(expectedX, x);
            Assert.Equal(expectedY, y);
            Assert.Equal(expectedDirection, direction);
        }

        public static TheoryData<string, int, int, Direction> TryParsePlaceArgsData() => new()
        {
            { "1,2,NORTH", 1, 2, Direction.North },
            { "8,5,SOUTH", 8, 5, Direction.South },
            { "99,12,EAST", 99, 12, Direction.East },
        };
    }
}
