using Xunit;

namespace ToyRobot.Tests
{
    public class RobotTests
    {
        [Theory]
        [MemberData(nameof(GetNextPositionData))]
        public void GetNextPosition_Should_Return_Correct_Position(
            int startX, int startY, Direction direction, int expectedX, int expectedY)
        {
            var robot = new Robot { XPos = startX, YPos = startY, Direction = direction };

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
            { 2, 2, Direction.Unset,  2, 2 }, // no movement when unset
        };
    }
}
