namespace Juggling;

public class RoughState
{
    /// <summary>
    /// State of each of the balls.
    /// Key: ball index
    /// Value: hand index (or null if ball is in the air)
    /// </summary>
    public required IReadOnlyDictionary<int, int?> BallStates { get; init; }
}
