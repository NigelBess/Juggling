using System.Numerics;

namespace Juggling;

public class HandAction
{
    public required Vector2 Position { get; init; }
    /// <summary>
    /// The ball being caught or thrown. If the hand is just moving, <see cref="Ball"/> will be ignored (even if the hand currently contains a ball)
    /// </summary>
    public int? Ball { get; init; }
    public HandMotionEndpoint? PopulatedMotion { get; set; }
    public BallThrow? PopulatedThrow { get; set; }
    private int? _frameIndex;
    public int FrameIndex
    {
        get
        {
            if (_frameIndex is null) throw new InvalidOperationException($"{nameof(FrameIndex)} has not been populated");
            return _frameIndex.Value;
        }
        set
        {
            _frameIndex = value;
        }
    }
    private int? _handIndex;
    public int HandIndex
    {
        get
        {
            if (_handIndex is null) throw new InvalidOperationException($"{nameof(HandIndex)} has not been populated");
            return _handIndex.Value;
        }
        set
        {
            _handIndex = value;
        }
    }
    public HandActionType ActionType { get; init; }
    private HandAction() { }
    public static HandAction Catch(float x, float y, int ball) => Catch(new(x, y), ball);
    public static HandAction Throw(float x, float y, int ball) => Throw(new(x, y), ball);
    public static HandAction Catch(Vector2 pos, int ball) => new() { Position = pos, ActionType = HandActionType.Catch, Ball = ball };
    public static HandAction Throw(Vector2 pos, int ball) => new() { Position = pos, ActionType = HandActionType.Throw, Ball = ball };

}


