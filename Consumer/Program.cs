using System.Diagnostics;
using ApiClient.Models;
using ApiClient.Models.Dtos;
using ApiClient.Services;
using ApiClient.Services.Abstractions;

await Execute();

async Task Execute()
{
    var client = new Client(new ClientConfiguration(new Uri("https://localhost:7051/")));
    var emails = Enumerable.Range(1, 15).Select(i => $"test{i}@pl.pl").ToList();
    var byOneResult = await ByOne(emails, client);
    var parallelResult = await Parallel(emails, client);
    Console.WriteLine($"By one time in seconds: {byOneResult.Time}");
    Console.WriteLine($"Parallel time in seconds: {parallelResult.Time}");
    Console.WriteLine("Press enter to print results");
    Console.ReadLine();
    Print(byOneResult.Items, "By one");
    Print(parallelResult.Items, "Parallel");
    Console.WriteLine("Press enter to quit");
    Console.ReadLine();
}

void Print(IEnumerable<GetUser> items, string title)
{
    Console.WriteLine();
    Console.WriteLine(title);
    foreach (var item in items)
    {
        Console.WriteLine(item.EMail);
    }
}

async Task<(IEnumerable<GetUser> Items, double Time)> ByOne(IEnumerable<string> emails, IClient client)
{
    var sw = new Stopwatch();
    var results = new List<GetUser>();
    sw.Start();
    foreach (var email in emails)
    {
        var result = await client.GetUserByEmail(email);
        if (result.IsT0)
        {
            results.Add(result.AsT0);
        }
    }
    sw.Stop();

    return (results, sw.Elapsed.TotalSeconds);
}

async Task<(IEnumerable<GetUser> Items, double Time)> Parallel(IEnumerable<string> emails, IClient client)
{
    var sw = new Stopwatch();
    sw.Start();
    var tasks = emails.Select(s => client.GetUserByEmail(s));
    var results = await Task.WhenAll(tasks);
    sw.Stop();

    return (results.Where(of => of.IsT0).Select(of => of.AsT0), sw.Elapsed.TotalSeconds);
}