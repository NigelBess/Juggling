using System.Diagnostics.CodeAnalysis;

namespace Juggling;

public static class Extension
{

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
    where T : class
    => items.Where(i => i is not null)!;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
    where T : struct
    => items.Where(i => i.HasValue).Select(i => i!.Value);

    /// <summary>
    /// Number of rows in the 2d Array (index 0)
    /// </summary>
    public static int Rows<T>(this T[,] map) => map.GetLength(dimension: 0);
    /// <summary>
    /// Number of columns in the 2d Array (index 1)
    /// </summary>
    public static int Columns<T>(this T[,] map) => map.GetLength(dimension: 1);

    /// <summary>
    /// Converts the enumerable to a string in the following format:
    /// "item1, item2, item3, ... , itemN"
    /// </summary>
    public static string ToCollectionString<T>(this IEnumerable<T> enumerable, string separator = ",", bool includeSpace = true) => $"{string.Join($"{separator}{(includeSpace ? " " : string.Empty)}", enumerable)}";

    /// <summary>
    /// Counts the number of occurences of each item in the enumerable
    /// </summary>
    public static Dictionary<T, int> GetCounts<T>(this IEnumerable<T> enumerable) where T : notnull => enumerable.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

    /// <summary>
    /// Sorts a list by a key selector
    /// </summary>
    public static void SortBy<T>(this List<T> list, Func<T, IComparable> keySelector)
    {
        list.Sort((a, b) => keySelector(a).CompareTo(keySelector(b)));
    }

    /// <summary>
    /// Attempts to find the only item in a sequence.
    /// </summary>
    /// <returns>false if sequence contains no elements or sequence contains more than one element</returns>
    public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, [NotNullWhen(returnValue: true)] out T? single) where T : notnull => enumerable.TryGetSingle(t => true, out single);
    /// <summary>
    /// Attempts to find the only item in a sequence matching the given condition.
    /// </summary>
    /// <returns>true if an one and only one item was found that matches the given condition</returns>
    public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, Predicate<T> condition, [NotNullWhen(returnValue: true)] out T? single) where T : notnull
    {
        var found = false;
        single = default(T?);
        foreach (var item in enumerable.Where(i => condition(i)))
        {
            if (found)
            { // already found one, and now we found another? -> fail
                single = default(T?);
                return false;
            }
            single = item;
            found = true;
        }
        return found;
    }

    /// <summary>
    /// Attempts to find the first item in a sequence.
    /// </summary>
    /// <returns>false if sequence contains no elements</returns>
    public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, [NotNullWhen(returnValue: true)] out T? first) where T : notnull => enumerable.TryGetFirst(t => true, out first);

    /// <summary>
    /// Attempts to find the first item in a sequence matching the given condition
    /// </summary>
    /// <returns>true if an item was found that matches the given condition</returns>
    public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, Predicate<T> condition, [NotNullWhen(returnValue: true)] out T? first) where T : notnull
    {
        foreach (var item in enumerable)
        {
            if (condition(item))
            {
                first = item;
                return true;
            }
        }
        first = default(T?);
        return false;
    }

    public static T[] Shift<T>(this T[] arr, int rightShift)
    {
        var len = arr.Length;
        var result = new T[len];
        var shift = ((rightShift % len) + len) % len;
        for (int i = 0; i < len; i++)
            result[(i + shift) % len] = arr[i];
        return result;
    }

    public static bool IsEven(this int n) => n % 2 == 0;
    public static bool IsOdd(this int n) => !n.IsEven();
}
