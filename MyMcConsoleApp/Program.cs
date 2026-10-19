using System.Diagnostics;
using System.Text.Json;
using MyMc;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file path as an argument.");
    return;
}

MyMcRegistration.Register();

var watch = new Stopwatch();
watch.Start();

using FileStream fileStream = new(args[0], FileMode.Open);
var memoryCard = MemoryCardOperations.Read(fileStream);

watch.Stop();

Console.WriteLine("Output:");
Console.WriteLine(memoryCard);
Console.WriteLine("Execution time: " + watch.ElapsedMilliseconds);
Console.WriteLine("Press any button to continue...");
Console.Read();
