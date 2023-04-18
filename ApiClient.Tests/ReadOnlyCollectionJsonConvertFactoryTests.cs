using System.Text.Json;
using Xunit;

namespace ApiClient.Tests;

public class ReadOnlyCollectionJsonConvertFactoryTests
{
    [Fact]
    public void WithNullShouldReturnCollection()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ReadOnlyCollectionJsonConvertFactory());
        var result = JsonSerializer.Deserialize<IReadOnlyCollection<string>>("null", options)!;
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public void WithEmptyShouldReturnCollection()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ReadOnlyCollectionJsonConvertFactory());
        var result = JsonSerializer.Deserialize<IReadOnlyCollection<string>>("[]", options)!;
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public void WithNonEmptyShouldReturnCollection()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ReadOnlyCollectionJsonConvertFactory());
        var result = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(@"[""i1"", ""i2""]", options)!;
        Assert.Equal(2, result.Count);
    }
}