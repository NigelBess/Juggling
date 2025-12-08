using System.Numerics;

namespace Juggling;

internal class HandMotionSequence : IMotionSequence
{
    private List<HandMotion> _subMotions;
    public HandMotionSequence(IEnumerable<HandMotion> subMotions)
    {
        _subMotions = subMotions.ToList();
    }
    public Vector2 GetPosition(float timeInFrames) => GetSubMotion(timeInFrames, out var localTime).GetPosition(localTime);

    private HandMotion GetSubMotion(float timeInFrames, out float localTime)
    {
        var elapsed = 0;
        foreach (var handMotion in _subMotions)
        {
            if (elapsed + handMotion.DurationFrames > timeInFrames)
            {
                localTime = timeInFrames - elapsed;
                return handMotion;
            }
            elapsed += handMotion.DurationFrames;
        }
        throw new InvalidOperationException($"Attempting to find hand position at time {timeInFrames} but pattern is only {elapsed} frames long");
    }
}
