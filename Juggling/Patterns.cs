using System.Numerics;

namespace Juggling;

public static class Patterns
{

    public static Pattern Default(int n)
    {
        if (n.IsEven()) return EvenBallsSeparate(n);
        return StandardOddBallPattern(n);
    }
    public static Pattern StandardOddBallPattern(int n, Standard2HandLayout? layout = null)
    {
        layout ??= Standard2HandLayout.Io();
        var leftActions = TwoHandIoPattern(n, layout.Left);
        var rightActions = TwoHandIoPattern(n, layout.Right).Shift(n);
        return Pattern.FromHands(leftActions, rightActions);
    }

    public static Pattern EvenBallsSeparate(int n, Standard2HandLayout? layout = null, int offset = 1)
    {
        if (!n.IsEven()) throw new InvalidOperationException($"This only works with an even number of balls. You specified {n} balls");
        layout ??= Standard2HandLayout.Io();
        var halfN = n / 2;
        var leftActions = SingleHand(halfN, layout.Left);
        var rightActions = SingleHand(halfN, layout.Right, ballOffset: halfN).Shift(offset);
        return Pattern.FromHands(leftActions, rightActions);
    }

    public static Pattern Tennis(Standard2HandLayout? layout = null, float outerBallAdditionalOffset = 60, float outerBallYOffset = 50)
    {
        layout ??= Standard2HandLayout.Io(20, 60);
        var (lo, li, ri, ro) = layout.SplitAsIo();
        var loo = lo + new Vector2(-outerBallAdditionalOffset, outerBallYOffset);
        var roo = ro + new Vector2(outerBallAdditionalOffset);
        HandAction?[] left =
            [
                HandAction.Catch(loo,0),
                HandAction.Throw(loo,0),
                HandAction.Catch(lo,1),
                HandAction.Throw(li,1),
                HandAction.Catch(lo,2),
                HandAction.Throw(li,2),
            ];
        HandAction?[] right =
            [
                HandAction.Throw(ri,1),
                HandAction.Catch(ro,2),
                HandAction.Throw(ri,2),
                HandAction.Catch(roo,0),
                HandAction.Throw(roo,0),
                HandAction.Catch(ri,1),
            ];
        return Pattern.FromHands(left, right);
    }

    public static Pattern DoubleMill(Standard2HandLayout? layout = null)
    {
        layout ??= Standard2HandLayout.Io();
        var (lo, li, ri, ro) = layout.SplitAsIo();
        HandAction?[] left =
            [
                HandAction.Catch(li,0),
                HandAction.Throw(lo,0),
                HandAction.Catch(li,1),
                HandAction.Throw(lo,1),
                HandAction.Catch(ri,2),
                HandAction.Throw(ro,2),
            ];
        HandAction?[] right =
            [
                HandAction.Throw(ro,1),
                HandAction.Catch(li,2),
                HandAction.Throw(lo,2),
                HandAction.Catch(ri,0),
                HandAction.Throw(ro,0),
                HandAction.Catch(ri,1),

            ];
        return Pattern.FromHands(left, right);
    }

    public static Pattern SingleHandPattern(int n) => Pattern.FromHands(SingleHand(n, Standard1HandLayout.Flat()));


    private static HandAction[] SingleHand(int n, Standard1HandLayout layout, int ballOffset = 0)
    {
        var actions = new HandAction[n * 2];
        for (var i = 0; i < n; i++)
        {
            actions[2 * i] = HandAction.Catch(layout.CatchPos, i + ballOffset);
            actions[2 * i + 1] = HandAction.Throw(layout.ThrowPos, i + ballOffset);

        }
        return actions;
    }

    private static HandAction[] TwoHandIoPattern(int n, Standard1HandLayout layout)
    {
        var actions = new HandAction[n * 2];
        for (var i = 0; i < n; i++)
        {
            actions[2 * i] = HandAction.Catch(layout.CatchPos, i);
            actions[2 * i + 1] = HandAction.Throw(layout.ThrowPos, i);
        }
        return actions;
    }
}
