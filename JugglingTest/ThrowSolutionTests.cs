using Juggling;

namespace JugglingTest;

[TestClass]
public class ThrowSolutionTests
{
    [TestMethod]
    public void TestPositionAtCatchAndThrow()
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
        var ballThrow = pattern.GenerateThrows().Single();
        var solution = ballThrow.GenerateSolution(10);
        var throwPos = solution.GetPosition(0);
        TestHelpers.AssertV2Equality(throwPos, ballThrow.Throw.Position, "Start");
        TestHelpers.AssertV2Equality(solution.EndPosition, ballThrow.Catch.Position, "End");
    }
}
