using ToyRobot;
using Xunit;

namespace ToyRobot.Tests
{
    public class SimulatorTests
    {
        [Fact]
        public void Place_Should_Successfully_Execute()
        {
            var simulator = new Simulator();
            var result = simulator.Place(1, 2, Direction.North);

            Assert.True(result);
            Assert.True(simulator.IsPlaced);
            Assert.Equal("1, 2, North", simulator.Report());
        }

        [Fact]
        public void Place_Should_Return_False_And_Not_Set_Robot()
        {
            var simulator = new Simulator();
            var result = simulator.Place(1, 6, Direction.North);

            Assert.False(result);
            Assert.False(simulator.IsPlaced);
        }

        [Fact]
        public void Move_Should_Move_Position_Forward()
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, Direction.North);

            var result = simulator.MoveForward();

            Assert.True(result);
            Assert.Equal("1, 2, North", simulator.Report());
        }

        [Fact]
        public void Move_Should_Not_Move_If_Going_Out_Of_Bounds()
        {
            var simulator = new Simulator();
            simulator.Place(0, 4, Direction.North);

            var result = simulator.MoveForward();

            Assert.False(result);
            Assert.Equal("0, 4, North", simulator.Report());
        }

        [Theory]
        [MemberData(nameof(TurnLeftData))]
        public void Turn_Left_Should_Turn_Left(Direction start, Direction expected)
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, start);

            simulator.TurnLeft();

            Assert.Equal($"1, 1, {expected}", simulator.Report());
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
        public void Turn_Right_Should_Turn_Right(Direction start, Direction expected)
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, start);

            simulator.TurnRight();

            Assert.Equal($"1, 1, {expected}", simulator.Report());
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
            var simulator = new Simulator();
            simulator.Place(x, y, direction);

            Assert.Equal(expected, simulator.Report());
        }

        public static TheoryData<int, int, Direction, string> ReportData() => new()
        {
            { 1, 2, Direction.South, "1, 2, South"},
            { 3, 3, Direction.West, "3, 3, West"},
            { 3, 1, Direction.North, "3, 1, North"},
        };

        [Fact]
        public void Report_Should_Throw_When_Robot_Not_Placed()
        {
            var simulator = new Simulator();

            Assert.Throws<InvalidOperationException>(() => simulator.Report());
        }

        [Fact]
        public void Place_Should_Update_Position_When_Placed_Again()
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, Direction.North);

            simulator.Place(3, 3, Direction.East);

            Assert.Equal("3, 3, East", simulator.Report());
        }
    }
}
