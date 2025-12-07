using System.Numerics;

namespace Juggling;

internal class BallThrow
{
    public required int LoopFrameCount { get; init; }
    public required HandAction Throw { get; init; }
    public required HandAction Catch { get; init; }
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
    public double? GetLocalFrameIndex(double globalFrameIndex)
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
    /// Solves for the start velocity
    /// </summary>
    public ThrowSolution GenerateSolution(Vector2 gravity)
    {
        var catchPos = Catch.Position;
        var throwPos = Throw.Position;
        var positionChange = catchPos - throwPos;

        return new();
    }

}
