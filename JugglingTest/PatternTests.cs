using Juggling;

namespace JugglingTest;

[TestClass]
public sealed class PatternTests
{
    [TestMethod]
    [DynamicData(nameof(GetPatterns))]
    public void TestSingleHandPatternValidity(HandPattern handPattern, bool expectedValid)
    {
        var pattern = new Pattern() { Hands = [handPattern] };
        Assert.AreEqual(expectedValid, pattern.TryGetStateSequence(out _, out var errorMessage), errorMessage);
    }

    private static IEnumerable<object[]> GetPatterns()
    {
        yield return new object[] { ValidPattern1(), true };
        yield return new object[] { ValidPattern2(), true };
        yield return new object[] { InvalidPattern1(), false };
        yield return new object[] { InvalidPattern2(), false };
        yield return new object[] { InvalidPattern3(), false };
    }

    private static HandPattern InvalidPattern1() => new()
    {
        Actions =
            [
                new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 } // Catch but no throw -> Invalid
            ]
    };
    private static HandPattern InvalidPattern2() => new()
    {
        // Catching more than you throw -> invalid
        Actions =
        [
            new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 },
            new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 1 },
            new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 }
        ]
    };
    private static HandPattern InvalidPattern3() => new()
    {
        // Throwing a different ball from the one you just caught
        Actions =
        [
            new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 },
            new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 2 },
            new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 2 },
            new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 1 },
        ]
    };

    private static HandPattern ValidPattern1() => new()
    {
        Actions =
         [
             new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 },
                null,
             new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 1 } // Catch and throw same ball repeatedly -> valid
         ]
    };
    private static HandPattern ValidPattern2() => new()
    {
        // 2 ball juggling
        Actions =
         [
             new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 1 },
             new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 1 },
             new() { X = 0, Y = 0, ActionType = HandActionType.Catch, Ball = 2 },
             new() { X = 0, Y = 0, ActionType = HandActionType.Throw, Ball = 2 },
         ]
    };
}
