using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Consumer;

public static class TelemetryExtensions
{
    public static async Task<TResult> GetValueWithActivityAsync<TResult, TParam>(this Activity? activity, Func<TParam, Task<TResult>> func, TParam param)
    {
        try
        {
            var result = await func(param).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(e);

            throw;
        }
    }

    public static async Task ExecuteWithActivityAsync<TParam>(this Activity? activity, Func<TParam, Task> func, TParam param)
    {
        try
        {
            await func(param).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(e);

            throw;
        }
    }

    public static TResult GetValueWithActivity<TResult, TParam>(this Activity? activity, Func<TParam, TResult> func, TParam param)
    {
        try
        {
            var result = func(param);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(e);

            throw;
        }
    }

    public static void ExecuteWithActivity<TParam>(this Activity? activity, Action<TParam> action, TParam param)
    {
        try
        {
            action(param);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(e);

            throw;
        }
    }
}