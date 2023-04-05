using System.Diagnostics;
using System.Text.Json;
using Consumer;

await Execute();

async Task Execute()
{
    var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7051/") };
    var emails = Enumerable.Range(1, 15).Select(i => $"test{i}@pl.pl").ToList();
    var byOneResult = await ByOne(emails, httpClient);
    var parallelResult = await Parallel(emails, httpClient);
    Console.WriteLine($"By one time in seconds: {byOneResult.Time}");
    Console.WriteLine($"Parallel time in seconds: {parallelResult.Time}");
    Console.WriteLine("Press enter to print results");
    Console.ReadLine();
    Print(byOneResult.Items, "By one");
    Print(parallelResult.Items, "Parallel");
    Console.WriteLine("Press enter to quit");
    Console.ReadLine();
}

void Print(IEnumerable<UserDto> items, string title)
{
    Console.WriteLine();
    Console.WriteLine(title);
    foreach (var item in items)
    {
        Console.WriteLine(item.EMail);
    }
}

async Task<UserDto> GetUserByEmail(string email, HttpClient client)
{
    var response = await client.GetAsync($"User/{email}");
    response.EnsureSuccessStatusCode();
    var contentAsString = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<UserDto>(contentAsString)!;
}

async Task<(IEnumerable<UserDto> Items, double Time)> ByOne(IEnumerable<string> emails, HttpClient client)
{
    var sw = new Stopwatch();
    var results = new List<UserDto>();
    sw.Start();
    foreach (var email in emails)
    {
        results.Add(await GetUserByEmail(email, client));
    }
    sw.Stop();

    return (results, sw.Elapsed.TotalSeconds);
}

async Task<(IEnumerable<UserDto> Items, double Time)> Parallel(IEnumerable<string> emails, HttpClient client)
{
    var sw = new Stopwatch();
    sw.Start();
    var tasks = emails.Select(s => GetUserByEmail(s, client));
    var results = await Task.WhenAll(tasks);
    sw.Stop();

    return (results, sw.Elapsed.TotalSeconds);
}