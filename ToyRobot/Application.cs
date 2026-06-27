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

                switch (parsed.Type)
                {
                    case CommandType.Place:
                        if (parsed.Options is { } opts)
                        {
                            if (_simulator.Place(opts.X, opts.Y, opts.Facing))
                                _output.WriteLine("Robot placed.");
                            else
                                _output.WriteLine("Invalid position — robot was not placed.");
                        }
                        else
                        {
                            _output.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
                        }
                        break;
                    case CommandType.Move:
                        if (_simulator.IsRobotPlaced())
                        {
                            if (_simulator.MoveForward())
                                _output.WriteLine("Moved forward.");
                            else
                                _output.WriteLine("Move blocked — robot is at the edge of the table.");
                        }
                        else
                        {
                            _output.WriteLine("Robot has not been placed yet.");
                        }
                        break;
                    case CommandType.Left:
                        if (_simulator.IsRobotPlaced())
                        {
                            _simulator.TurnLeft();
                            _output.WriteLine("Turned left.");
                        }
                        else
                        {
                            _output.WriteLine("Robot has not been placed yet.");
                        }
                        break;
                    case CommandType.Right:
                        if (_simulator.IsRobotPlaced())
                        {
                            _simulator.TurnRight();
                            _output.WriteLine("Turned right.");
                        }
                        else
                        {
                            _output.WriteLine("Robot has not been placed yet.");
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
