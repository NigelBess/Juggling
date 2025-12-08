using Juggling;

namespace JugglingTest;

[TestClass]
public class HandMotionTests
{
    [TestMethod]
    public void TestSolutionIsAccurate()
    {
        var pattern = Patterns.StandardOddBallPattern(3);
        var handMotions = pattern.GetHandMotions(-100);
        foreach (var handMotionSequence in handMotions)
        {
            var currentState = handMotionSequence.Last().End;
            foreach (var motion in handMotionSequence)
            {
                var startState = motion.Start;
                const float delta = 1e-4f;
                const float velocityEpsilon = 1e-5f;
                const float velocityDelta = 0.5f;
                var nextPos = motion.GetPosition(velocityEpsilon);
                var initialvelocity = (nextPos - startState.Position) / velocityEpsilon;
                TestHelpers.AssertEquality(startState.Velocity, initialvelocity, delta: velocityDelta, "Start velocity");
                TestHelpers.AssertEquality(currentState, startState);
                TestHelpers.AssertEquality(startState.Position, motion.GetPosition(0), delta: delta);
                TestHelpers.AssertEquality(motion.End.Position, motion.GetPosition(motion.DurationFrames), delta: delta);
                currentState = motion.End;
            }
        }
    }

    [TestMethod]
    public void TestSubMotionIndexing()
    {
        var pattern = Patterns.StandardOddBallPattern(3);
        var handMotions = pattern.GetHandMotions(-100);
        var dtFrames = 0.01f;
        for (float tFrames = 0; tFrames < pattern.FrameCount; tFrames += dtFrames)
        {
            foreach (var motion in handMotions)
            {
                var subMotion = motion.GetSubMotion(tFrames, out var localTime);
                Assert.IsGreaterThanOrEqualTo(0, localTime);
                Assert.IsLessThan(subMotion.DurationFrames, localTime);
            }
        }
    }

}
