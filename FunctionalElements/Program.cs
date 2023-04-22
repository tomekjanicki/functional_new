using FunctionalElements;
using FunctionalElements.JsonConverters;
using FunctionalElements.Services;
using FunctionalElements.Types;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("Open telemetry POC service"))
    .WithTracing(providerBuilder => providerBuilder
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.SetDbStatementForStoredProcedure = true;
        })
        .AddConsoleExporter()
        .AddJaegerExporter());

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new WithValueJsonConvertFactory());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapTypeToString<NonEmpty50CharsString>();
    options.MapTypeToString<EMail>();
    options.MapTypeToInteger<UserId>();
});
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();