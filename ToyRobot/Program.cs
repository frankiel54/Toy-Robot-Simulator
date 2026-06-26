
bool placed = false;
Console.WriteLine("Toy Robot app");
while (true)
{
    Console.WriteLine("Here are the valid commands:");
    Console.WriteLine("PLACE X,Y,Z");
    Console.WriteLine("MOVE");
    Console.WriteLine("LEFT");
    Console.WriteLine("RIGHT");
    Console.WriteLine("REPORT");
    
    if (!placed) { 
        Console.WriteLine("Please enter an initial place command");
    }

    string command = Console.ReadLine();

    switch (command)
    {
        case "PLACE":
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