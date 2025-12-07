using Juggling;

namespace JugglingTest;

[TestClass]
public sealed class PatternTests
{



    [TestMethod]
    public void InvalidPattern1_IsInvalid() => TestSingleHandPatternValidity(InvalidPattern1(), false);

    [TestMethod]
    public void InvalidPattern2_IsInvalid() => TestSingleHandPatternValidity(InvalidPattern2(), false);
    [TestMethod]
    public void SingleBallToss_IsValid() => TestSingleHandPatternValidity(SingleBallToss(), true);

    [TestMethod]
    public void SingleHandTwoBallJuggle_IsValid() => TestSingleHandPatternValidity(SingleHandTwoBallJuggle(), true);

    [TestMethod]
    public void ThrowTwoBallsFromTheSameHand_IsValid() => TestSingleHandPatternValidity(ThrowTwoBallsFromTheSameHand(), true);

    [TestMethod]
    public void SingleBallToss_OneThrow() => TestThrowCount(SingleBallToss(), 1);

    [TestMethod]
    public void SingleHandTwoBallJuggle_TwoThrows() => TestThrowCount(SingleHandTwoBallJuggle(), 2);
    [TestMethod]
    public void OneBallWithPauses_OneThrow() => TestThrowCount(OneBallWithPauses(), 1);


    public void TestThrowCount(HandPattern handPattern, int expectedThrowCount)
    {
        var pattern = new Pattern() { Hands = [handPattern] };
        var throws = pattern.GenerateThrows();
        Assert.HasCount(expectedThrowCount, throws);
    }

    public void TestSingleHandPatternValidity(HandPattern handPattern, bool expectedValid)
    {
        var pattern = new Pattern() { Hands = [handPattern] };
        Assert.AreEqual(expectedValid, pattern.TryGetStateSequence(out _, out var errorMessage), errorMessage);
    }

    private static HandPattern InvalidPattern1() => new()
    {
        Actions =
            [
                HandAction.Catch(0,0,1) // Catch but no throw -> Invalid
            ]
    };
    private static HandPattern InvalidPattern2() => new()
    {
        // Catching more than you throw -> invalid
        Actions =
        [
            HandAction.Catch(0,0,1),
            HandAction.Throw(0,0,1),
            HandAction.Catch(0,0,1)
        ]
    };
    private static HandPattern ThrowTwoBallsFromTheSameHand() => new()
    {
        // Throwing a different ball from the one you just caught
        Actions =
        [
            HandAction.Catch(0,0,1),
            HandAction.Throw(0,0,2),
            HandAction.Catch(0,0,2),
            HandAction.Throw(0,0,1),
        ]
    };

    private static HandPattern OneBallWithPauses() => new()
    {
        // Throwing a different ball from the one you just caught
        Actions = [
                    null,
                    HandAction.Catch(0,0,1),
                    null,
                    HandAction.Throw(0,0,1),
                ]
    };

    private static HandPattern SingleBallToss() => new()
    {
        Actions =
         [
             HandAction.Catch(0,0,1),
                null,
             HandAction.Throw(0,0,1) // Catch and throw same ball repeatedly -> valid
         ]
    };
    private static HandPattern SingleHandTwoBallJuggle() => new()
    {
        // 2 ball juggling
        Actions =
         [
             HandAction.Catch(0,0,1),
             HandAction.Throw(0,0,1),
             HandAction.Catch(0,0,2),
             HandAction.Throw(0,0,2),
         ]
    };
}
