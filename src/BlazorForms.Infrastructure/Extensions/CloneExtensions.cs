using System.Text.Json;

namespace BlazorForms.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for creating deep copies of objects.
/// </summary>
public static class CloneExtensions
{
    /// <summary>
    /// Creates a deep copy of the specified object by serializing and deserializing it.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="source">The object to clone.</param>
    /// <returns>A deep copy of the specified object.</returns>
    public static T DeepClone<T>(this T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
