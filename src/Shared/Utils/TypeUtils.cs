using System.Collections.Concurrent;
using Shared.CQRS.Decorators;

namespace Shared.Utils;

public static class TypeUtils
{
    private static readonly ConcurrentDictionary<Type, string> _typeNameCache = new();

    public static string GetInnermostDecoratedName(IDecorator decorator)
    {
        return _typeNameCache.GetOrAdd(decorator.GetType(), _ =>
        {
            object value = decorator;

            while (value is IDecorator currentDecorator)
            {
                var decorated = currentDecorator.Decorated;

                if (decorated == null)
                {
                    return value.GetType().Name;
                }

                value = decorated;
            }

            return value.GetType().Name;
        });
    }
}
