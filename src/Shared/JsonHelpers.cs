using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public static class JsonHelpers
{
    public static JsonSerializerOptions DefaultOptions { get; }

    static JsonHelpers()
    {
        DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        DefaultOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
