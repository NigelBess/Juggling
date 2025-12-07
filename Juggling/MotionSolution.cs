using System.Numerics;

namespace Juggling;

internal class MotionSolution
{
    private readonly float _a;
    private readonly float _b;
    private readonly float _c;
    private readonly float _d;
    private readonly float _duration;

    public MotionSolution(float y0, float v0, float y1, float v1, float duration)
    {
        _duration = duration;

        var T = duration;
        var T2 = T * T;
        var T3 = T2 * T;

        _a = (T * (v0 + v1) + 2f * y0 - 2f * y1) / T3;
        _b = (-2f * T * v0 - T * v1 - 3f * y0 + 3f * y1) / T2;
        _c = v0;
        _d = y0;
    }

    public static MotionSolution FromMotionEndpoints(HandMotionEndpoint start, HandMotionEndpoint end, Func<Vector2, float> indexSelector, float duration) => new(indexSelector(start.Position), indexSelector(start.Velocity), indexSelector(end.Position), indexSelector(end.Velocity), duration)

    public float GetPosition(float t)
    {
        if (t <= 0f) return _d;
        if (t >= _duration)
        {
            var T = _duration;
            return ((_a * T + _b) * T + _c) * T + _d;
        }

        return ((_a * t + _b) * t + _c) * t + _d;
    }
}
