using Xunit;

namespace ToyRobot.Tests
{
    public class RobotTests
    {
        [Fact]
        public void PlaceAt_Should_Set_All_Properties()
        {
            var robot = new Robot();
            robot.PlaceAt(2, 3, Direction.South);

            Assert.Equal(2, robot.XPos);
            Assert.Equal(3, robot.YPos);
            Assert.Equal(Direction.South, robot.Direction);
        }

        [Fact]
        public void MoveTo_Should_Update_Position_And_Preserve_Direction()
        {
            var robot = new Robot(1, 1, Direction.North);
            robot.MoveTo(3, 4);

            Assert.Equal(3, robot.XPos);
            Assert.Equal(4, robot.YPos);
            Assert.Equal(Direction.North, robot.Direction);
        }

        [Fact]
        public void TurnLeft_Should_Rotate_Direction()
        {
            var robot = new Robot(0, 0, Direction.North);
            robot.TurnLeft();

            Assert.Equal(Direction.West, robot.Direction);
        }

        [Fact]
        public void TurnRight_Should_Rotate_Direction()
        {
            var robot = new Robot(0, 0, Direction.North);
            robot.TurnRight();

            Assert.Equal(Direction.East, robot.Direction);
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
            { 2, 2, Direction.Unset,  2, 2 },
        };
    }
}
