namespace Juggling;

public class HandMotion
{
    private readonly MotionSolution _x;
    private readonly MotionSolution _y;
    public int DurationFrames { get; }
    public HandMotionEndpoint Start { get; }
    public HandMotionEndpoint End { get; }
    public HandMotion(HandMotionEndpoint start, HandMotionEndpoint end, int durationFrames)
    {
        Start = start;
        End = end;
        DurationFrames = durationFrames;
        _x = MotionSolution.FromMotionEndpoints(start, end, v => v.X, durationFrames);
        _y = MotionSolution.FromMotionEndpoints(start, end, v => v.Y, durationFrames);
    }
}
