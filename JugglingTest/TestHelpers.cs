using Juggling;
using System.Numerics;

namespace JugglingTest;

internal static class TestHelpers
{
    public static void AssertEquality(Vector2 v1, Vector2 v2, float delta = 1e-6f, string additionalInfo = "")
    {
        Assert.AreEqual(v1.X, v2.X, delta, $"Mismatch in X dimension {additionalInfo}");
        Assert.AreEqual(v1.Y, v2.Y, delta, $"Mismatch in Y dimension {additionalInfo}");
    }

    public static void AssertEquality(HandMotionEndpoint e1, HandMotionEndpoint e2, float delta = 1e-6f, string additionalInfo = "")
    {
        AssertEquality(e1.Position, e2.Position, delta, $"Mismatch in Position {additionalInfo}");
        AssertEquality(e1.Velocity, e2.Velocity, delta, $"Mismatch in Velocity {additionalInfo}");
    }
}
