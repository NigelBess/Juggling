using Juggling;

namespace JugglingTest;

[TestClass]
public sealed class PatternTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetPatterns), DynamicDataSourceType.Method)]
    public void TestIsValid(Pattern pattern, bool expectedValid)
    {
        Assert.AreEqual(expectedValid, pattern.TryGetStateSequence(out _, out var errorMessage), errorMessage);
    }

    private static IEnumerable<object[]> GetPatterns()
    {
        yield return new object[] { ValidPattern(), true };
        yield return new object[] { InvalidPattern(), false };
    }

    private static Pattern InvalidPattern()
    {
        var handPattern = new HandPattern()
        {
            Actions = new()[
                new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 }
            ]
        };
    }
    private static Pattern ValidPattern() => new(1, 100);
}
