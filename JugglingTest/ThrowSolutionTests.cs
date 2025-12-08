using Juggling;

namespace JugglingTest;

[TestClass]
public class ThrowSolutionTests
{
    private BallThrow GenerateOneBallToss()
    {
        var pattern = new Pattern()
        {
            Hands = new() {
                new(){
                    Actions = [
                        null,
                        HandAction.Catch(0,0,1),
                        null,
                        HandAction.Throw(0,0,1),
                    ]
                }
            }
        };
        return pattern.GenerateThrows().Single();
    }
    [TestMethod]
    public void TestPositionAtCatchAndThrow()
    {
        var ballThrow = GenerateOneBallToss();
        var solution = ballThrow.ComputeSolution(-10);
        var throwPos = solution.GetPosition(0);
        TestHelpers.AssertEquality(throwPos, ballThrow.Throw.Position, additionalInfo: "Start");
        TestHelpers.AssertEquality(solution.EndPosition, ballThrow.Catch.Position, additionalInfo: "End");
    }

    [TestMethod]
    public void TestSignErrors()
    {
        var ballThrow = GenerateOneBallToss();
        var solution = ballThrow.ComputeSolution(-10);
        var zenith = solution.Zenith.Y;
        Assert.IsGreaterThan(0, zenith);
        var range = solution.YRange();
        Assert.AreEqual(0, range.Min);
        Assert.AreEqual(zenith, range.Max);

    }
}
