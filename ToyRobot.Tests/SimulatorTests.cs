using System.Reflection;
using System.Runtime.InteropServices;
using ToyRobot;
using Xunit;
using Xunit.v3;

namespace ToyRobot.Tests
{
    public class SimulatorTests
    {
        [Fact]
        public void Place_Should_Successfully_Execute()
        {
            var robot = new Robot ();
            var gameBoard = new Simulator(robot);
            var result = gameBoard.Place(1,2, Direction.North);

            Assert.Equal(Direction.North, robot.direction);
            Assert.Equal(1, robot.xPos);
            Assert.Equal(2, robot.yPos);
            Assert.True(result);
        }

        [Fact]
        public void Place_Should_Return_False_And_Not_Set_Robot()
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            var result = gameBoard.Place(1, 6, Direction.North);

            Assert.Equal(Direction.Unset, robot.direction);
            Assert.Equal(-1, robot.xPos);
            Assert.Equal(-1, robot.yPos);
            Assert.False(result);
        }

        [Fact]
        public void Move_Should_Move_Position_Forward()
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            gameBoard.Place(1, 1, Direction.North);

            var result = gameBoard.MoveForward();

            Assert.Equal(Direction.North, robot.direction);
            Assert.Equal(1, robot.xPos);
            Assert.Equal(2, robot.yPos);
            Assert.True(result);
        }

        [Fact]
        public void Move_Should_Not_Move_If_Going_Out_Of_Bounds()
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            gameBoard.Place(0, 4, Direction.North);

            var result = gameBoard.MoveForward();

            Assert.Equal(Direction.North, robot.direction);
            Assert.Equal(0, robot.xPos);
            Assert.Equal(4, robot.yPos);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(TurnLeftData))]
        public void Turn_Left_Should_Turn_Left(Direction start, Direction expected)
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            gameBoard.Place(1, 1, start);

            gameBoard.TurnLeft();

            Assert.Equal(expected, robot.direction);
            Assert.Equal(1, robot.xPos);
            Assert.Equal(1, robot.yPos);
        }

        public static TheoryData<Direction, Direction> TurnLeftData() => new()
        {
            { Direction.East, Direction.North },
            { Direction.North, Direction.West },
            { Direction.West, Direction.South },
            { Direction.South, Direction.East },
        };

        [Theory]
        [MemberData(nameof(TurnRightData))]
        public void Turn_Left_Should_Turn_Right(Direction start, Direction expected)
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            gameBoard.Place(1, 1, start);

            gameBoard.TurnRight();

            Assert.Equal(expected, robot.direction);
            Assert.Equal(1, robot.xPos);
            Assert.Equal(1, robot.yPos);
        }

        public static TheoryData<Direction, Direction> TurnRightData() => new()
        {
            { Direction.East, Direction.South },
            { Direction.South, Direction.West },
            { Direction.West, Direction.North },
            { Direction.North, Direction.East },
        };


        [Theory]
        [MemberData(nameof(ReportData))]
        public void Report(int x, int y, Direction direction, string expected)
        {
            var robot = new Robot();

            var gameBoard = new Simulator(robot);
            gameBoard.Place(x, y, direction);


            Assert.Equal(expected, gameBoard.Report());
        }

        public static TheoryData<int, int, Direction, string> ReportData() => new()
        {
            { 1, 2, Direction.South, "1, 2, South"},
            { 3, 3, Direction.West, "3, 3, West"},
            { 3, 1, Direction.North, "3, 1, North"},
        };

    }
}
