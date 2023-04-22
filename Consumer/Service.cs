using System.Diagnostics;
using ApiClient.Models;
using ApiClient.Services.Abstractions;
using ApiClient.Services;
using ApiClient;
using ApiClient.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace Consumer;

public sealed class Service
{
    private readonly ILogger<Service> _logger;

    public Service(ILogger<Service> logger)
    {
        _logger = logger;
    }

    private static async Task WithExceptionActivityNotHandled()
    {
        using var activity = Telemetry.ActivitySource.StartActivity();
        await activity.ExecuteWithActivityAsync(static p => WithExceptionInt(p), activity);
    }

    private async Task WithExceptionActivityHandled()
    {
        try
        {
            using var activity = Telemetry.ActivitySource.StartActivity();
            await activity.ExecuteWithActivityAsync(static p => WithExceptionInt(p), activity);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception");
        }
    }

    private static async Task WithExceptionInt(Activity? p)
    {
        p?.SetTag("TestTag", "bla");
        if (DateTime.Now.Year == 2000)
        {
            await Task.Delay(10);
        }

        throw new InvalidOperationException("My custom exception");
    }

    public async Task Execute2()
    {
        Console.WriteLine("Press enter to print start");
        Console.ReadLine();
        var client = new Client(new ClientConfiguration(new Uri("https://localhost:7051/")));
        var parallel = await Parallel(new List<string> { "test1@pl.pl", "test2@pl.pl", "test3@pl.pl" }, client);
        Print(parallel, "Parallel");
        Console.WriteLine("Press enter to execute");
        Console.ReadLine();
        await RootActivity().ConfigureAwait(false);
        Console.WriteLine("Press enter to quit");
        Console.ReadLine();
    }

    private async Task RootActivity()
    {
        using var activity = Telemetry.ActivitySource.StartActivity();
        await activity.ExecuteWithActivityAsync(async _ =>
        {
            await WithExceptionActivityHandled().ConfigureAwait(false);
            await WithExceptionActivityNotHandled().ConfigureAwait(false);
        }, false).ConfigureAwait(false);
    }

    public async Task Execute(int items)
    {
        Console.WriteLine("Press enter to print start");
        Console.ReadLine();
        var client = new Client(new ClientConfiguration(new Uri("https://localhost:7051/")));
        var emails = Enumerable.Range(1, items).Select(i => $"test{i}@pl.pl").ToList();
        var byOneResult = await ByOne(emails, client);
        var parallelResult = await Parallel(emails, client);
        Console.WriteLine("Press enter to print results");
        Console.ReadLine();
        Print(byOneResult, "By one");
        Print(parallelResult, "Parallel");
        Console.WriteLine("Press enter to quit");
        Console.ReadLine();
    }

    private static void Print(IEnumerable<GetUser> items, string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        foreach (var item in items)
        {
            Console.WriteLine(item.EMail);
        }
    }

    private async Task<IEnumerable<GetUser>> ByOne(IEnumerable<string> emails, IClient client)
    {
        using var activity = Telemetry.ActivitySource.StartActivity();
        var results = new List<GetUser>();
        foreach (var email in emails)
        {
            _logger.LogInformation("Starting request for email {email}.", email);
            var result = await client.GetUserByEmail(email);
            _logger.LogInformation("Finish request for email {email}.", email);
            results.Add(result.GetResultIfSuccessOrThrow());
        }
        activity?.SetTag("total", results.Count);
        activity?.SetStatus(ActivityStatusCode.Ok);
        return results;
    }

    private static async Task<IReadOnlyCollection<GetUser>> Parallel(IEnumerable<string> emails, IClient client)
    {
        using var activity = Telemetry.ActivitySource.StartActivity();

        return await activity.GetValueWithActivityAsync(async static p =>
        {
            var tasks = p.emails.Select(s => p.client.GetUserByEmail(s));
            var results = await Task.WhenAll(tasks);
            p.activity?.SetTag("total", results.Length);
            return results.Select(of => of.GetResultIfSuccessOrThrow()).ToArray();

        }, (client, emails, activity));
    }
}