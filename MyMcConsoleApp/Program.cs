using MyMc;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file path as an argument.");
    return;
}

var bytes = await File.ReadAllBytesAsync(args[0]);
var memoryCard = await new Operations().ReadMemoryCard(bytes);

Console.WriteLine("Version: " + string.Join(", ", memoryCard.Version));
Console.WriteLine("Games: " + string.Join(", ", memoryCard.Games));
Console.WriteLine("Press any button to continue...");
Console.Read();