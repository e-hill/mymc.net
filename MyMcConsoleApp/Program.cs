using MyMc;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file path as an argument.");
    return;
}

using FileStream fileStream = new(args[0], FileMode.Open);
var memoryCard = Operations.ReadMemoryCard(fileStream);

Console.WriteLine("Press any button to continue...");
Console.Read();