namespace Juggling;

public class RoughState
{
    /// <summary>
    /// State of each of the balls.
    /// Key: ball index
    /// Value: hand index (or null if ball is in the air)
    /// </summary>
    public required IReadOnlyDictionary<int, int?> BallStates { get; init; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not RoughState other) return false;

        if (BallStates.Count != other.BallStates.Count) return false;

        foreach (var kvp in BallStates)
        {
            if (!other.BallStates.TryGetValue(kvp.Key, out var otherValue))
                return false;

            if (!EqualityComparer<int?>.Default.Equals(kvp.Value, otherValue))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hash = 17;

        foreach (var kvp in BallStates)
        {
            hash ^= HashCode.Combine(kvp.Key, kvp.Value);
        }

        return hash;
    }
    public override string ToString() => BallStates
    .Select(kvp => $"{{Ball{kvp.Key}:{(kvp.Value is null ? "Air" : kvp.Value)}}}")
    .ToCollectionString();

}
