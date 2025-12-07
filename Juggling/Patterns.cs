using System.Numerics;

namespace Juggling;

public static class Patterns
{
    private static readonly Vector2 lo = new(-75, 0);
    private static readonly Vector2 li = new(-25, 0);
    private static readonly Vector2 ro = new(75, 0);
    private static readonly Vector2 ri = new(25, 0);
    public static Pattern IO3() => new()
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

    public static Pattern IO5() => new()
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
}
