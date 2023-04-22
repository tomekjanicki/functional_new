using System.Diagnostics;

namespace Consumer;

public static class Telemetry
{
    public const string ServiceName = "Open telemetry POC consumer";

    public const string ActivitySourceName = "My activity";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}