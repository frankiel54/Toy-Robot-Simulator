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
        public void Place_Should_Successfully_Execute ()
        {
            var gameBoard = new GameBoard(5,5, new Robot());
            var result = gameBoard.Place(1,1, Direction.North);

            Assert.True(result);
        }
    }
}
