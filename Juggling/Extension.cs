namespace Juggling;

public static class Extension
{
    public static Dictionary<T, int> GetCounts<T>(this IEnumerable<T> items) where T : notnull
    {
        var counts = new Dictionary<T, int>();
        foreach (var item in items)
        {
            if (!counts.TryGetValue(item, out var c))
            {
                c = 0;
            }
            c++;
            counts[item] = c;
        }
        return counts;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
    where T : class
    => items.Where(i => i is not null)!;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
    where T : struct
    => items.Where(i => i.HasValue).Select(i => i!.Value);

    public static string ToCollectionString<T>(this IEnumerable<T> items) => string.Join(", ", items);
}
