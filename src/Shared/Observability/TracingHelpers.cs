using Serilog;
using SerilogTracing;

namespace Shared.Observability;

public static class TracingHelpers
{
    public static IDisposable StartActivity(string activityName)
    {
        return Log.Logger.StartActivity(activityName);
    }
}
