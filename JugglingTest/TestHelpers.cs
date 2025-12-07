using System.Numerics;

namespace JugglingTest;

internal static class TestHelpers
{
    public static void AssertV2Equality(Vector2 v1, Vector2 v2, string additionalInfo = "", float delta = 1e-6f)
    {
        Assert.AreEqual(v1.X, v2.X, delta, $"Mismatch in X dimension {additionalInfo}");
        Assert.AreEqual(v1.Y, v2.Y, delta, $"Mismatch in Y dimension {additionalInfo}");
    }
}
