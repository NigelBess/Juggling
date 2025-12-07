using System.Numerics;

namespace Juggling;

internal class ThrowSolution
{
    /// <summary>
    /// Gravitational acceleration in distance units per frame^2
    /// </summary>
    public required Vector2 Gravity { get; init; }
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
    public required Vector2 EndVelocity { get; init; }

    public Vector2 GetPosition(double localFrameIndex)
    {

    }

}
