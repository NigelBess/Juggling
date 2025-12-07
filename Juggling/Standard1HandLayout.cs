using System.Numerics;


namespace Juggling;

public class Standard1HandLayout
{
    public required Vector2 ThrowPos { get; init; }
    public required Vector2 CatchPos { get; init; }
    public static Standard1HandLayout Flat(float throwPos = 25, float catchPos = 75) => new()
    {
        ThrowPos = Vector2Helpers.X(throwPos),
        CatchPos = Vector2Helpers.X(catchPos),
    };

    public Standard1HandLayout MirrorHorizontally() => new()
    {
        ThrowPos = new(-ThrowPos.X, ThrowPos.Y),
        CatchPos = new(-CatchPos.X, CatchPos.Y),
    };
}
