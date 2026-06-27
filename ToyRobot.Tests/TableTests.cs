using Xunit;

namespace ToyRobot.Tests
{
    public class TableTests
    {
        [Theory]
        [MemberData(nameof(ValidPositionData))]
        public void IsValidPosition_Should_ReturnTrue_ForPositionsInsideBounds(int x, int y)
        {
            var table = new Table();

            Assert.True(table.IsValidPosition(x, y));
        }

        public static TheoryData<int, int> ValidPositionData() => new()
        {
            { 0, 0 },   // bottom-left corner
            { 4, 4 },   // top-right corner (5x5, indices 0-4)
            { 0, 4 },   // top-left corner
            { 4, 0 },   // bottom-right corner
            { 2, 2 },   // centre
        };

        [Theory]
        [MemberData(nameof(InvalidPositionData))]
        public void IsValidPosition_Should_ReturnFalse_ForPositionsOutsideBounds(int x, int y)
        {
            var table = new Table();

            Assert.False(table.IsValidPosition(x, y));
        }

        public static TheoryData<int, int> InvalidPositionData() => new()
        {
            { -1,  0 },  // x below lower bound
            {  0, -1 },  // y below lower bound
            { -1, -1 },  // both below lower bound
            {  5,  0 },  // x at width (out of range)
            {  0,  5 },  // y at height (out of range)
            {  5,  5 },  // both at dimension boundary
            { 10, 10 },  // far out of range
        };

        [Fact]
        public void IsValidPosition_Should_RespectCustomDimensions()
        {
            var table = new Table(width: 3, height: 2);

            Assert.True(table.IsValidPosition(2, 1));   // last valid cell
            Assert.False(table.IsValidPosition(3, 1));  // x == width
            Assert.False(table.IsValidPosition(2, 2));  // y == height
        }
    }
}
