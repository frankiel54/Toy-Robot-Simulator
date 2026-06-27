namespace ToyRobot
{
    public class Application
    {
        private readonly Simulator _simulator = new(new Robot());
        private readonly TextReader _input;
        private readonly TextWriter _output;

        public Application() : this(Console.In, Console.Out) { }

        public Application(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
        }

        public void Run()
        {
            _output.WriteLine("Toy Robot app");
            _output.WriteLine("Here are the valid commands:");
            _output.WriteLine("PLACE X,Y,Z");
            _output.WriteLine("MOVE");
            _output.WriteLine("LEFT");
            _output.WriteLine("RIGHT");
            _output.WriteLine("REPORT");

            while (true)
            {
                string? line = _input.ReadLine();
                if (line is null) break;

                var parsed = CommandParser.ParseCommand(line);

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
                            _output.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
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
                            _output.WriteLine(_simulator.Report());
                        }
                        break;
                    default:
                        _output.WriteLine("\nInvalid selection. Please try again.");
                        break;
                }
            }
        }
    }
}
