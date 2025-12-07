using Juggling;

namespace JugglingTest;

[TestClass]
public class RoughStateTests
{
    [TestMethod]
    public void TestEqual()
    {
        var rs1 = new RoughState() { BallStates = new Dictionary<int, int?>() { { 1, null } } };
        var rs2 = new RoughState() { BallStates = new Dictionary<int, int?>() { { 1, null } } };
        Assert.AreEqual(rs1, rs2);
    }

    [TestMethod]
    public void TestNotEqual()
    {
        var rs1 = new RoughState() { BallStates = new Dictionary<int, int?>() { { 1, null } } };
        var rs2 = new RoughState() { BallStates = new Dictionary<int, int?>() { { 1, 0 } } };
        Assert.AreNotEqual(rs1, rs2);
    }
}
