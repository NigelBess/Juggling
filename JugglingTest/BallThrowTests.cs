using Juggling;

namespace JugglingTest;

[TestClass]
public class BallThrowTests
{
    [TestMethod]
    [DataRow(0.3f, 1.3f)]
    [DataRow(1.3f, null)]
    [DataRow(2.3f, null)]
    [DataRow(3.3f, 0.3f)]
    public void TestLocalFrameIndex(float globalIndex, float? expectedLocalIndex)
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
        var localIndex = ballThrow.GetLocalFrameIndex(globalIndex);
        if (expectedLocalIndex is null)
        {
            Assert.IsNull(localIndex);
        }
        else
        {
            Assert.IsNotNull(localIndex);
            Assert.AreEqual(expectedLocalIndex.Value, localIndex!.Value, delta: 1e-6f);
        }

    }
}
