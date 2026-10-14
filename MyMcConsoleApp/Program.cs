using System.Diagnostics;
using MyMc;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file path as an argument.");
    return;
}

var watch = new Stopwatch();
watch.Start();

using FileStream fileStream = new(args[0], FileMode.Open);
var memoryCard = Operations.ReadMemoryCard(fileStream);

watch.Stop();

Console.WriteLine("Execution time: " + watch.ElapsedMilliseconds);
Console.WriteLine("Press any button to continue...");
Console.Read();