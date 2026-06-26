
using ToyRobot;

var GameBoard = new GameBoard(4,4, new Robot());

Console.WriteLine("Toy Robot app");
Console.WriteLine("Here are the valid commands:");
Console.WriteLine("PLACE X,Y,Z");
Console.WriteLine("MOVE");
Console.WriteLine("LEFT");
Console.WriteLine("RIGHT");
Console.WriteLine("REPORT");

while (true)
{
    //if (!) { 
    //    Console.WriteLine("Please enter an initial place command");
    //}

    string input = Console.ReadLine() ?? string.Empty;
    var parts = input.Split(' ', 2);
    string command = parts[0].ToUpper();
    string commandArgs = parts.Length > 1 ? parts[1] : string.Empty;

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
            break;
        case "LEFT":
            break;
        case "RIGHT":
            break;
        case "REPORT":
            break;
        default:
            Console.WriteLine("\nInvalid selection. Please try again.");
            break;
    }
}