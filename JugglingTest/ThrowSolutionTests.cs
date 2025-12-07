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
        var solution = ballThrow.ComputeSolution(10);
        var throwPos = solution.GetPosition(0);
        TestHelpers.AssertV2Equality(throwPos, ballThrow.Throw.Position, "Start");
        TestHelpers.AssertV2Equality(solution.EndPosition, ballThrow.Catch.Position, "End");
    }

    [TestMethod]
    public void TestSignErrors()
    {
        var ballThrow = GenerateOneBallToss();
        var solution = ballThrow.ComputeSolution(10);

    }
}
