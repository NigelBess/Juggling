namespace Juggling;

public class Standard2HandLayout
{
    public required Standard1HandLayout Left { get; init; }
    public required Standard1HandLayout Right { get; init; }

    public static Standard2HandLayout Oi(float innerOffset = 25, float outerOffset = 75)
    {
        var right = Standard1HandLayout.Flat(throwPos: outerOffset, catchPos: innerOffset);
        var left = right.MirrorHorizontally();
        return new() { Left = left, Right = right };
    }

    public static Standard2HandLayout Io(float innerOffset = 25, float outerOffset = 75) => Oi(innerOffset: outerOffset, outerOffset: innerOffset);
}
