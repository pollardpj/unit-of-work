﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Utils;

public static class JsonHelpers
{
    public static JsonSerializerOptions DefaultOptions { get; }

    static JsonHelpers()
    {
        DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        DefaultOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
