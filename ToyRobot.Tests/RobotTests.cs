using Xunit;

namespace ToyRobot.Tests
{
    public class RobotTests
    {
        [Fact]
        public void Constructor_Should_Initialize_All_Properties()
        {
            var robot = new Robot(2, 3, Direction.South);

            Assert.Equal(2, robot.XPos);
            Assert.Equal(3, robot.YPos);
            Assert.Equal(Direction.South, robot.Direction);
        }

        [Fact]
        public void With_Expression_Should_Update_Position_And_Preserve_Direction()
        {
            var robot = new Robot(1, 1, Direction.North);
            var moved = robot with { XPos = 3, YPos = 4 };

            Assert.Equal(3, moved.XPos);
            Assert.Equal(4, moved.YPos);
            Assert.Equal(Direction.North, moved.Direction);
        }

        [Fact]
        public void With_Expression_TurnLeft_Should_Rotate_Direction()
        {
            var robot = new Robot(0, 0, Direction.North);
            var turned = robot with { Direction = robot.Direction.TurnLeft() };

            Assert.Equal(Direction.West, turned.Direction);
        }

        [Fact]
        public void With_Expression_TurnRight_Should_Rotate_Direction()
        {
            var robot = new Robot(0, 0, Direction.North);
            var turned = robot with { Direction = robot.Direction.TurnRight() };

            Assert.Equal(Direction.East, turned.Direction);
        }

        [Theory]
        [MemberData(nameof(GetNextPositionData))]
        public void GetNextPosition_Should_Return_Correct_Position(
            int startX, int startY, Direction direction, int expectedX, int expectedY)
        {
            var robot = new Robot(startX, startY, direction);

            var (x, y) = robot.GetNextPosition();

            Assert.Equal(expectedX, x);
            Assert.Equal(expectedY, y);
        }

        public static TheoryData<int, int, Direction, int, int> GetNextPositionData() => new()
        {
            { 2, 2, Direction.North,  2, 3 },
            { 2, 2, Direction.South,  2, 1 },
            { 2, 2, Direction.East,   3, 2 },
            { 2, 2, Direction.West,   1, 2 },
        };
    }
}
