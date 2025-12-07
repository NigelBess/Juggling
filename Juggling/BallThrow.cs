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

    public ThrowSolution? PopulatedSolution { get; set; }

    public IEnumerable<HandAction> Actions => [Throw, Catch];
    public int FrameCount
    {
        get
        {
            if (IsAroundTheCorner)
            {
                return (LoopFrameCount - Throw.FrameIndex) + Catch.FrameIndex;
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

        if (IsAroundTheCorner)
        {
            if (globalFrameIndex > Catch.FrameIndex && globalFrameIndex < Throw.FrameIndex) return null;
            if (globalFrameIndex < Catch.FrameIndex) return LoopFrameCount - Throw.FrameIndex + globalFrameIndex;
            return globalFrameIndex - Throw.FrameIndex;
        }
        if (globalFrameIndex > Catch.FrameIndex || globalFrameIndex < Throw.FrameIndex) return null;
        return globalFrameIndex - Throw.FrameIndex;
    }

    public ThrowSolution ComputeSolution(float gravityDistancePerFrameSquared) => new(this, gravityDistancePerFrameSquared);

    public void PopulateSolution(float gravityDistancePerFrameSquared) => PopulatedSolution = ComputeSolution(gravityDistancePerFrameSquared);
}
