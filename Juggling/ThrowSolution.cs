using System.Numerics;

namespace Juggling;

internal class ThrowSolution
{
    /// <summary>
    /// Gravitational acceleration in distance units per frame^2
    /// </summary>
    public required float Gravity { get; init; }
    /// <summary>
    /// How long the throw takes (in frames)
    /// </summary>
    public required int Time { get; init; }
    /// <summary>
    /// Position from which throw begins (distance units)
    /// </summary>
    public required Vector2 StartPosition { get; init; }
    /// <summary>
    /// Velocity in distance units per frame
    /// </summary>
    public required Vector2 StartVelocity { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public required Vector2 EndPosition { get; init; }
    /// <summary>
    /// Velocity in distance units per frame
    /// </summary>
    public Vector2 EndVelocity => new(
        StartVelocity.X,
        StartVelocity.Y + Gravity * Time
    );

    public Vector2 GetPosition(float localFrame)
    {
        return new Vector2(
            StartPosition.X + StartVelocity.X * localFrame,                       // x(t) = x0 + vx0 * t
            StartPosition.Y + StartVelocity.Y * localFrame + 0.5f * Gravity * localFrame * localFrame     // y(t) = y0 + vy0 * t + 0.5 * a * t^2
        );
    }

}
