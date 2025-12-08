using System.Collections;
using System.Numerics;

namespace Juggling;

public class HandMotionSequence : IMotionSequence, IEnumerable<HandMotion>
{
    private List<HandMotion> _subMotions;
    public HandMotionSequence(IEnumerable<HandMotion> subMotions)
    {
        _subMotions = subMotions.ToList();
    }

    public IEnumerator<HandMotion> GetEnumerator() => _subMotions.GetEnumerator();

    public Vector2 GetPosition(float timeInFrames) => GetSubMotion(timeInFrames, out var localTime).GetPosition(localTime);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public HandMotion GetSubMotion(float timeInFrames, out float localTime)
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
