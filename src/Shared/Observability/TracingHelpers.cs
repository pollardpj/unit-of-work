using Serilog;
using SerilogTracing;

namespace Shared.Observability;

public static class TracingHelpers
{
    public static IDisposable StartActivity(string activity, params object[] propertyValues)
    {
        return Log.Logger.StartActivity(activity, propertyValues);
    }
}
