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
        public void ParseCommand_Should_Parse(string rawCommand, CommandType expectedType)
        {
            var result = CommandParser.ParseCommand(rawCommand);

            Assert.Equal(expectedType, result.Type);
        }

        public static TheoryData<string, CommandType> ParseCommandData() => new()
        {
            { "PLACE 1,1,NORTH", CommandType.Place },
            { "MOVE",            CommandType.Move },
            { "LEFT",            CommandType.Left },
            { "RIGHT",           CommandType.Right },
            { "REPORT",          CommandType.Report },
            { "COMMAND1",        CommandType.Unknown },
        };

        [Theory]
        [MemberData(nameof(TryParsePlaceArgsData))]
        public void TryParsePlaceArgs_Should_Parse(string rawArgs, int expectedX, int expectedY, Direction expectedDirection)
        {
            CommandParser.TryParsePlaceArgs(rawArgs, out int x, out int y, out Direction direction);

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

        [Theory]
        [MemberData(nameof(ParsePlaceCommandData))]
        public void ParseCommand_Should_Parse_Place_Options(string rawCommand, int expectedX, int expectedY, Direction expectedFacing)
        {
            var result = CommandParser.ParseCommand(rawCommand);

            Assert.Equal(CommandType.Place, result.Type);
            Assert.NotNull(result.Options);
            Assert.Equal(expectedX, result.Options.X);
            Assert.Equal(expectedY, result.Options.Y);
            Assert.Equal(expectedFacing, result.Options.Facing);
        }

        public static TheoryData<string, int, int, Direction> ParsePlaceCommandData() => new()
        {
            { "PLACE 1,2,NORTH", 1, 2, Direction.North },
            { "PLACE 3,4,EAST",  3, 4, Direction.East },
        };

        [Fact]
        public void ParseCommand_Should_Return_Place_With_Null_Options_For_Invalid_Args()
        {
            var result = CommandParser.ParseCommand("PLACE bad_args");

            Assert.Equal(CommandType.Place, result.Type);
            Assert.Null(result.Options);
        }
    }
}
