using System.Text.Json;
using ApiClient.V2.Models;
using Xunit;

namespace ApiClient.Tests.V2.Models;

public class AlarmTests
{
    [Fact]
    public void ShouldSerializeAndDeserialize()
    {
        var source = new Alarm
        {
            Id = "id1",
            CustomFragments = new Dictionary<string, object?>
            {
                { "k1", "v1" },
                { "k2", 5 },
                { "k3", new Composite(12, "test") },
                { "k4", null }
            }
        };
        Alarm.TryAddProperty("k3", typeof(Composite));
        var jsonString = JsonSerializer.Serialize(source);
        var deserialized = JsonSerializer.Deserialize<Alarm>(jsonString);
        Assert.NotNull(deserialized);
        Assert.Equal(source.Id, deserialized.Id);
        Assert.Equal(source.CustomFragments.Count, deserialized.CustomFragments.Count);
    }

    public sealed class Composite
    {
        public Composite(int f1, string f2)
        {
            F1 = f1;
            F2 = f2;
        }

        public int F1 { get; }

        public string F2 { get; }
    }
}