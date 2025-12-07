using System.Numerics;

namespace Juggling;

public static class Patterns
{
    private static readonly Vector2 lo = new(-75, 0);
    private static readonly Vector2 li = new(-25, 0);
    private static readonly Vector2 ro = new(75, 0);
    private static readonly Vector2 ri = new(25, 0);
    public static Pattern Io3() => new()
    {
        Hands = new() {
            new() {
                Actions = [
                    HandAction.Throw(li,0),
                    HandAction.Catch(lo,1),
                    HandAction.Throw(li,1),
                    HandAction.Catch(lo,2),
                    HandAction.Throw(li,2),
                    HandAction.Catch(lo,0),
                ]
            },
            new() {
                Actions = [
                    HandAction.Catch(ro,2),
                    HandAction.Throw(ri,2),
                    HandAction.Catch(ro,0),
                    HandAction.Throw(ri,0),
                    HandAction.Catch(ro,1),
                    HandAction.Throw(ri,1),
                ]
            }
        }
    };

    public static Pattern Io5() => new()
    {
        Hands = new() {
            new() {
                Actions = [
                    HandAction.Throw(li,0),
                    HandAction.Catch(lo,1),
                    HandAction.Throw(li,1),
                    HandAction.Catch(lo,2),
                    HandAction.Throw(li,2),
                    HandAction.Catch(lo,3),
                    HandAction.Throw(li,3),
                    HandAction.Catch(lo,4),
                    HandAction.Throw(li,4),
                    HandAction.Catch(lo,0),
                ]
            },
            new() {
                Actions = [
                    HandAction.Catch(ro,3),
                    HandAction.Throw(ri,3),
                    HandAction.Catch(ro,4),
                    HandAction.Throw(ri,4),
                    HandAction.Catch(ro,0),
                    HandAction.Throw(ri,0),
                    HandAction.Catch(ro,1),
                    HandAction.Throw(ri,1),
                    HandAction.Catch(ro,2),
                    HandAction.Throw(ri,2),
                ]
            }
        }
    };

    public static Pattern Io(int n)
    {
        var leftActions = new HandAction?[2 * n];
        var rightActions = new HandAction?[2 * n];
        var shift = (n + 1) / 2; // ceil(n/2)

        for (var i = 0; i < n; i++)
        {
            // Left hand: Throw(li, i), then Catch(lo, i+1 mod n)
            leftActions[2 * i] = HandAction.Throw(li, i);
            leftActions[2 * i + 1] = HandAction.Catch(lo, (i + 1) % n);

            // Right hand: Catch/Throw in rotated ball order, starting with middle index ball
            var ball = (i + shift) % n;
            rightActions[2 * i] = HandAction.Catch(ro, ball);
            rightActions[2 * i + 1] = HandAction.Throw(ri, ball);
        }

        return new()
        {
            Hands = new()
        {
            new()
            {
                Actions = leftActions
            },
            new()
            {
                Actions = rightActions
            }
        }
        };
    }
}
