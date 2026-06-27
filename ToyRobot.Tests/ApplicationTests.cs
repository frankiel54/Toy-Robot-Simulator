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
        public void Place_Valid_Should_Place_Robot()
        {
            var result = Run("PLACE 1,2,NORTH", "REPORT");

            Assert.Contains("1, 2, North", result);
        }

        [Fact]
        public void Place_Invalid_Args_Should_Print_Error()
        {
            var result = Run("PLACE bad_input");

            Assert.Contains("Invalid PLACE arguments", result);
        }

        [Fact]
        public void Move_Before_Place_Should_Not_Crash_Or_Report_Position()
        {
            var result = Run("MOVE", "REPORT");

            Assert.DoesNotContain("0, 0", result);
            Assert.DoesNotContain("-1", result);
        }

        [Fact]
        public void Move_Should_Advance_Position()
        {
            var result = Run("PLACE 1,1,NORTH", "MOVE", "REPORT");

            Assert.Contains("1, 2, North", result);
        }

        [Fact]
        public void Move_Should_Not_Go_Out_Of_Bounds()
        {
            var result = Run("PLACE 0,4,NORTH", "MOVE", "REPORT");

            Assert.Contains("0, 4, North", result);
        }

        [Fact]
        public void Left_Should_Rotate_Direction()
        {
            var result = Run("PLACE 2,2,NORTH", "LEFT", "REPORT");

            Assert.Contains("2, 2, West", result);
        }

        [Fact]
        public void Right_Should_Rotate_Direction()
        {
            var result = Run("PLACE 2,2,NORTH", "RIGHT", "REPORT");

            Assert.Contains("2, 2, East", result);
        }

        [Fact]
        public void Report_Before_Place_Should_Produce_No_Output()
        {
            var result = Run("REPORT");

            Assert.DoesNotContain("-1", result);
            Assert.DoesNotContain("Unset", result);
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
