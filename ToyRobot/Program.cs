
using ToyRobot;

Console.WriteLine("Toy Robot app");
Console.WriteLine("Here are the valid commands:");
Console.WriteLine("PLACE X,Y,Z");
Console.WriteLine("MOVE");
Console.WriteLine("LEFT");
Console.WriteLine("RIGHT");
Console.WriteLine("REPORT");

var GameBoard = new Simulator(new Robot());

while (true)
{
    // TODO: Make some initial message that only show once
    //if (!) { 
    //    Console.WriteLine("Please enter an initial place command");
    //}

    CommandParser.ParseCommand(Console.ReadLine() ?? string.Empty, out string command, out string commandArgs);


    // TODO: Add some more error messages for when things dont go right
    switch (command)
    {
        case "PLACE":
            if (CommandParser.TryParsePlaceArgs(commandArgs, out int x, out int y, out Direction direction))
            {
                GameBoard.Place(x, y, direction);
            }
            else
            {
                Console.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
            }
            break;
        case "MOVE":
            if (GameBoard.IsRobotPlaced()) {
                GameBoard.MoveForward();
            }
            break;
        case "LEFT":
            if (GameBoard.IsRobotPlaced())
            {
                GameBoard.TurnLeft();
            }
            break;
        case "RIGHT":
            if (GameBoard.IsRobotPlaced())
            {
                GameBoard.TurnRight();
            }
            break;
        case "REPORT":
            if (GameBoard.IsRobotPlaced())
            {
                var report = GameBoard.Report();
                Console.WriteLine(report);
            }
            break;
        default:
            Console.WriteLine("\nInvalid selection. Please try again.");
            break;
    }
}