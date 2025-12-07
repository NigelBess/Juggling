namespace Juggling;

public class HandAction
{
    public required double X { get; init; }
    public required double Y { get; init; }
    /// <summary>
    /// The ball being caught or thrown. If the hand is just moving, <see cref="Ball"/> will be ignored (even if the hand currently contains a ball)
    /// </summary>
    public int? Ball { get; init; }
    public required int Frame { get; init; }

    public HandActionType ActionType { get; init; }

}


