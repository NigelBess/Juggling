using System.Numerics;

namespace Juggling;

public interface IMotionSequence
{
    Vector2 GetPosition(float timeInFrames);
}
