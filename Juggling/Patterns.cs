using System.Numerics;

namespace Juggling;

public static class Patterns
{
    private static readonly Vector2 lo = new(-75, 0);
    private static readonly Vector2 li = new(-25, 0);
    private static readonly Vector2 ro = new(75, 0);
    private static readonly Vector2 ri = new(25, 0);

    public static Pattern Io(int n)
    {
        var leftActions = SingleHandIoPattern(n, lo, li);
        var rightActions = SingleHandIoPattern(n, ro, ri).Shift(n);
        return Pattern.FromHands(leftActions, rightActions);
    }



    private static HandAction[] SingleHandIoPattern(int n, Vector2 catchPosition, Vector2 throwPosition)
    {
        var actions = new HandAction[n * 2];
        for (var i = 0; i < n; i++)
        {
            actions[2 * i] = HandAction.Catch(catchPosition, i);
            actions[2 * i + 1] = HandAction.Throw(throwPosition, i);
        }
        return actions;
    }
}
