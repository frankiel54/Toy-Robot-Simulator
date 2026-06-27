using Xunit;

namespace ToyRobot.Tests
{
    public class ApplicationTests
    {
        private static string Run(params string[] lines)
        {
            var input  = new StringReader(string.Join("\n", lines));
            var output = new StringWriter();
            new Application(input, output).Run();
            return output.ToString();
        }

        [Fact]
        public void Run_Should_Print_Header_On_Startup()
        {
            var result = Run();

            Assert.Contains("Toy Robot app", result);
            Assert.Contains("PLACE X,Y,Z", result);
        }

        [Fact]
        public void Place_Valid_Should_Print_Success_And_Place_Robot()
        {
            var result = Run("PLACE 1,2,NORTH", "REPORT");

            Assert.Contains("Robot placed.", result);
            Assert.Contains("1, 2, North", result);
        }

        [Fact]
        public void Place_Out_Of_Bounds_Should_Print_Error()
        {
            var result = Run("PLACE 9,9,NORTH");

            Assert.Contains("Invalid position", result);
        }

        [Fact]
        public void Place_Invalid_Args_Should_Print_Error()
        {
            var result = Run("PLACE bad_input");

            Assert.Contains("Invalid PLACE arguments", result);
        }

        [Fact]
        public void Move_Before_Place_Should_Print_Not_Placed_Message()
        {
            var result = Run("MOVE");

            Assert.Contains("Robot has not been placed yet.", result);
        }

        [Fact]
        public void Move_Should_Print_Success_And_Advance_Position()
        {
            var result = Run("PLACE 1,1,NORTH", "MOVE", "REPORT");

            Assert.Contains("Moved forward.", result);
            Assert.Contains("1, 2, North", result);
        }

        [Fact]
        public void Move_Should_Print_Blocked_When_At_Edge()
        {
            var result = Run("PLACE 0,4,NORTH", "MOVE");

            Assert.Contains("Move blocked", result);
        }

        [Fact]
        public void Left_Before_Place_Should_Print_Not_Placed_Message()
        {
            var result = Run("LEFT");

            Assert.Contains("Robot has not been placed yet.", result);
        }

        [Fact]
        public void Left_Should_Print_Feedback_And_Rotate_Direction()
        {
            var result = Run("PLACE 2,2,NORTH", "LEFT", "REPORT");

            Assert.Contains("Turned left.", result);
            Assert.Contains("2, 2, West", result);
        }

        [Fact]
        public void Right_Before_Place_Should_Print_Not_Placed_Message()
        {
            var result = Run("RIGHT");

            Assert.Contains("Robot has not been placed yet.", result);
        }

        [Fact]
        public void Right_Should_Print_Feedback_And_Rotate_Direction()
        {
            var result = Run("PLACE 2,2,NORTH", "RIGHT", "REPORT");

            Assert.Contains("Turned right.", result);
            Assert.Contains("2, 2, East", result);
        }

        [Fact]
        public void Report_Before_Place_Should_Print_Not_Placed_Message()
        {
            var result = Run("REPORT");

            Assert.Contains("Robot has not been placed yet.", result);
        }

        [Fact]
        public void Unknown_Command_Should_Print_Invalid_Selection()
        {
            var result = Run("FOOBAR");

            Assert.Contains("Invalid selection", result);
        }

        [Fact]
        public void Full_Sequence_Should_Produce_Correct_Final_Position()
        {
            // PLACE 0,0,NORTH → MOVE (0,1,N) → RIGHT (0,1,E) → MOVE (1,1,E) → REPORT
            var result = Run("PLACE 0,0,NORTH", "MOVE", "RIGHT", "MOVE", "REPORT");

            Assert.Contains("1, 1, East", result);
        }
    }
}
