using System;
using System.Collections.Concurrent;

namespace Shared.Utils;

public static class TypeUtils
{
    private static readonly ConcurrentDictionary<Type, string> _typeNameCache = new();

    public static string GetUnderlyingTypeName(Type type)
    {
        return _typeNameCache.GetOrAdd(type, t =>
        {
            while (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments();
                if (genericArgs.Length == 0) break;
                t = genericArgs[0];
            }
            return t.Name;
        });
    }
} 