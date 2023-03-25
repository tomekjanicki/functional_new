using FunctionalElements;
using FunctionalElements.JsonConverters;
using FunctionalElements.Services;
using FunctionalElements.Types;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new WithValueJsonConvertFactory());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapTypeToString<NonEmpty50CharsString>();
    options.MapTypeToString<EMail>();
});
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();