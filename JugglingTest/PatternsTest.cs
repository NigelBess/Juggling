using Juggling;

namespace JugglingTest;

[TestClass]
public class PatternsTest
{
    [TestMethod]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(9)]
    [DataRow(11)]
    [DataRow(13)]
    public void TestIo(int n)
    {
        var pattern = Patterns.Io(n);
        var throws = pattern.GenerateThrows();
        var throwLengths = throws.Select(t => t.FrameCount).ToHashSet();
        Assert.HasCount(1, throwLengths); // all throws have the same length
        Assert.HasCount(n * 2, throws);

    }
    [TestMethod]
    [DataRow(3, 5)]
    public void TestIoHeight(int n, float expectedHeight)
    {
        var pattern = Patterns.Io(n);
        var throws = pattern.GenerateThrows();
        var height = throws.First().ComputeSolution(-10).Zenith.Y;
        Assert.AreEqual(expectedHeight, height);
    }
}
