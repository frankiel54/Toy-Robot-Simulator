namespace ToyRobot
{
    public class Application
    {
        private readonly Simulator _simulator = new(new Robot());

        public void Run()
        {
            Console.WriteLine("Toy Robot app");
            Console.WriteLine("Here are the valid commands:");
            Console.WriteLine("PLACE X,Y,Z");
            Console.WriteLine("MOVE");
            Console.WriteLine("LEFT");
            Console.WriteLine("RIGHT");
            Console.WriteLine("REPORT");

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
                            _simulator.Place(opts.X, opts.Y, opts.Facing);
                        }
                        else
                        {
                            Console.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
                        }
                        break;
                    case CommandType.Move:
                        if (_simulator.IsRobotPlaced())
                        {
                            _simulator.MoveForward();
                        }
                        break;
                    case CommandType.Left:
                        if (_simulator.IsRobotPlaced())
                        {
                            _simulator.TurnLeft();
                        }
                        break;
                    case CommandType.Right:
                        if (_simulator.IsRobotPlaced())
                        {
                            _simulator.TurnRight();
                        }
                        break;
                    case CommandType.Report:
                        if (_simulator.IsRobotPlaced())
                        {
                            Console.WriteLine(_simulator.Report());
                        }
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Please try again.");
                        break;
                }
            }
        }
    }
}
