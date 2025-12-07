using System.Numerics;

namespace Juggling;

public static class Vector2Helpers
{
    public static Vector2 X(float x) => new(x, 0);
    public static Vector2 Y(float y) => new(0, y);
}
