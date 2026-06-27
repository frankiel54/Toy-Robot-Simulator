
using ToyRobot;

Console.WriteLine("Toy Robot app");
Console.WriteLine("Here are the valid commands:");
Console.WriteLine("PLACE X,Y,Z");
Console.WriteLine("MOVE");
Console.WriteLine("LEFT");
Console.WriteLine("RIGHT");
Console.WriteLine("REPORT");

var Simulator = new Simulator(new Robot());

while (true)
{
    // TODO: Make some initial message that only show once
    //if (!) { 
    //    Console.WriteLine("Please enter an initial place command");
    //}

    var parsed = CommandParser.ParseCommand(Console.ReadLine() ?? string.Empty);

    // TODO: Add some more error messages for when things dont go right
    switch (parsed.Type)
    {
        case CommandType.Place:
            if (parsed.Options is { } opts)
            {
                Simulator.Place(opts.X, opts.Y, opts.Facing);
            }
            else
            {
                Console.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
            }
            break;
        case CommandType.Move:
            if (Simulator.IsRobotPlaced()) {
                Simulator.MoveForward();
            }
            break;
        case CommandType.Left:
            if (Simulator.IsRobotPlaced())
            {
                Simulator.TurnLeft();
            }
            break;
        case CommandType.Right:
            if (Simulator.IsRobotPlaced())
            {
                Simulator.TurnRight();
            }
            break;
        case CommandType.Report:
            if (Simulator.IsRobotPlaced())
            {
                var report = Simulator.Report();
                Console.WriteLine(report);
            }
            break;
        default:
            Console.WriteLine("\nInvalid selection. Please try again.");
            break;
    }
}