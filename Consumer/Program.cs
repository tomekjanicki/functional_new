using Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddOpenTelemetry(options =>
    {
        var resourceBuilder = ResourceBuilder.CreateDefault();
        resourceBuilder.AddService(Telemetry.ServiceName);
        options.SetResourceBuilder(resourceBuilder);
        options.IncludeScopes = true;
        options.AddConsoleExporter();
    });
});
builder.ConfigureServices(collection =>
{
    collection.AddOpenTelemetry()
        .ConfigureResource(resourceBuilder => resourceBuilder.AddService(Telemetry.ServiceName))
        .WithTracing(providerBuilder => providerBuilder
            .SetErrorStatusOnException()
            .AddHttpClientInstrumentation()
            .AddSource(Telemetry.ActivitySourceName)
            .AddConsoleExporter()
            .AddJaegerExporter());
    collection.AddSingleton<Service>();
});

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var hostStarted = false;
try
{
    await host.StartAsync();
    hostStarted = true;
    var service = host.Services.GetRequiredService<Service>();
    await service.Execute2();
}
catch (Exception e)
{
    logger.LogError(e, "Generic error handler");
    Console.WriteLine("Error occurred. Press enter to close.");
    Console.ReadLine();
}
if (hostStarted)
{
    await host.StopAsync();
}