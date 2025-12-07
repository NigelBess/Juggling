namespace Juggling;

public class BallThrow
{
    public required int LoopFrameCount { get; init; }
    public required HandAction Throw { get; init; }
    public required HandAction Catch { get; init; }
    public int Ball => Throw.Ball!.Value;
    /// <summary>
    /// Does this throw start at the end of the loop with the catch at the beginning?
    /// </summary>
    private bool IsAroundTheCorner => Catch.FrameIndex < Throw.FrameIndex;

    private int FrameCount
    {
        get
        {
            if (IsAroundTheCorner)
            {
                return (LoopFrameCount - Throw.FrameIndex) + Catch.FrameIndex + 1;
            }
            return (Catch.FrameIndex - Throw.FrameIndex);
        }
    }

    /// <summary>
    /// How many frames have elapsed since the start of this throw? <br/>
    /// Null if the throw is not in progress <br/>
    /// Accounts for "around-the-corner" throws (over looped pattern boundaries) if applicable.<br/>
    /// Fractional frames are allowed <br/>
    /// </summary>
    public float? GetLocalFrameIndex(float globalFrameIndex)
    {
        if (globalFrameIndex > Catch.FrameIndex && globalFrameIndex < Throw.FrameIndex) return null;
        if (IsAroundTheCorner)
        {
            if (globalFrameIndex < Catch.FrameIndex) return LoopFrameCount - Throw.FrameIndex + globalFrameIndex + 1;
            return globalFrameIndex - Throw.FrameIndex;
        }
        return globalFrameIndex - Throw.FrameIndex;
    }

    /// <summary>
    /// Solves the physics of the throw
    /// </summary>
    /// <param name="gravity">Gravity in distance units per frame squared. Expected to be positive for downward gravity.</param>
    public ThrowSolution GenerateSolution(float gravity)
    {
        var catchPos = Catch.Position;
        var throwPos = Throw.Position;
        var positionChange = catchPos - throwPos;
        var time = (float)FrameCount;
        var verticalInitialVelocity = (catchPos.Y - throwPos.Y + gravity * time * time / 2) / time;
        var horizontalInitialVelocity = (catchPos.X - throwPos.X) / time;

        return new()
        {
            Gravity = gravity,
            Time = FrameCount,
            StartVelocity = new(x: horizontalInitialVelocity, y: (float)verticalInitialVelocity),
            StartPosition = throwPos,
            EndPosition = catchPos,
        };
    }

}
