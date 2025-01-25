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

            while (value is IDecorator { Decorated: not null } currentDecorator)
            {
                value = currentDecorator.Decorated;
            }

            return value.GetType().Name;
        });
    }
}
