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
            Assert.Equal(1, simulator.X);
            Assert.Equal(2, simulator.Y);
            Assert.Equal(Direction.North, simulator.Facing);
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
            Assert.Equal(1, simulator.X);
            Assert.Equal(2, simulator.Y);
            Assert.Equal(Direction.North, simulator.Facing);
        }

        [Fact]
        public void Move_Should_Not_Move_If_Going_Out_Of_Bounds()
        {
            var simulator = new Simulator();
            simulator.Place(0, 4, Direction.North);

            var result = simulator.MoveForward();

            Assert.False(result);
            Assert.Equal(0, simulator.X);
            Assert.Equal(4, simulator.Y);
            Assert.Equal(Direction.North, simulator.Facing);
        }

        [Theory]
        [MemberData(nameof(TurnLeftData))]
        public void Turn_Left_Should_Turn_Left(Direction start, Direction expected)
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, start);

            simulator.TurnLeft();

            Assert.Equal(1, simulator.X);
            Assert.Equal(1, simulator.Y);
            Assert.Equal(expected, simulator.Facing);
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

            Assert.Equal(1, simulator.X);
            Assert.Equal(1, simulator.Y);
            Assert.Equal(expected, simulator.Facing);
        }

        public static TheoryData<Direction, Direction> TurnRightData() => new()
        {
            { Direction.East, Direction.South },
            { Direction.South, Direction.West },
            { Direction.West, Direction.North },
            { Direction.North, Direction.East },
        };

        [Theory]
        [MemberData(nameof(PlaceData))]
        public void Place_Should_Set_State(int x, int y, Direction direction)
        {
            var simulator = new Simulator();
            simulator.Place(x, y, direction);

            Assert.Equal(x, simulator.X);
            Assert.Equal(y, simulator.Y);
            Assert.Equal(direction, simulator.Facing);
        }

        public static TheoryData<int, int, Direction> PlaceData() => new()
        {
            { 1, 2, Direction.South },
            { 3, 3, Direction.West },
            { 3, 1, Direction.North },
        };

        [Fact]
        public void X_Should_Throw_When_Robot_Not_Placed()
        {
            var simulator = new Simulator();

            Assert.Throws<InvalidOperationException>(() => simulator.X);
        }

        [Fact]
        public void Place_Should_Update_Position_When_Placed_Again()
        {
            var simulator = new Simulator();
            simulator.Place(1, 1, Direction.North);

            simulator.Place(3, 3, Direction.East);

            Assert.Equal(3, simulator.X);
            Assert.Equal(3, simulator.Y);
            Assert.Equal(Direction.East, simulator.Facing);
        }
    }
}
