using System.Reflection;
using System.Runtime.InteropServices;
using ToyRobot;
using Xunit;
using Xunit.v3;

namespace ToyRobot.Tests
{
    public class GameBoardTests
    {
        [Fact]
        public void Place_Should_Successfully_Execute()
        {
            var robot = new Robot ();
            var gameBoard = new GameBoard(4 ,4, robot);
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

            var gameBoard = new GameBoard(4, 4, robot);
            var result = gameBoard.Place(1, 6, Direction.North);

            Assert.Equal(Direction.Unset, robot.direction);
            Assert.Equal(-1, robot.xPos);
            Assert.Equal(-1, robot.yPos);
            Assert.False(result);
        }
    }
}
