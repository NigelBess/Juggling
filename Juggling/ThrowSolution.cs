using System.Numerics;

namespace Juggling;

public class ThrowSolution
{
    /// <summary>
    /// Gravitational acceleration in distance units per frame^2
    /// Negative is in the down direction.
    /// </summary>
    public float Gravity { get; }
    /// <summary>
    /// How long the throw takes (in frames)
    /// </summary>
    public int Time { get; }
    /// <summary>
    /// Position from which throw begins (distance units)
    /// </summary>
    public Vector2 StartPosition { get; }
    /// <summary>
    /// Velocity in distance units per frame
    /// </summary>
    public Vector2 StartVelocity { get; }
    /// <summary>
    /// 
    /// </summary>
    public Vector2 EndPosition => GetPosition(Time);
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
            StartPosition.Y - StartVelocity.Y * localFrame + 0.5f * Gravity * localFrame * localFrame     // y(t) = y0 + vy0 * t + 0.5 * a * t^2
        );
    }

    public Vector2 Zenith
    {
        get
        {
            // Time at which vertical velocity is zero
            float zenithTime = StartVelocity.Y / Gravity;

            // Clamp to the duration of the throw
            zenithTime = MathF.Min(Time, MathF.Max(0f, zenithTime));
            return GetPosition(zenithTime);
        }
    }

    /// <summary>
    /// Generates a solution for a throw
    /// </summary>
    /// <param name="ballThrow"></param>
    /// <param name="gravity"></param>
    public ThrowSolution(BallThrow ballThrow, float gravityDistancePerFrameSquared)
    {
        var catchPos = ballThrow.Catch.Position;
        var throwPos = ballThrow.Throw.Position;
        var positionChange = catchPos - throwPos;
        var frameCount = ballThrow.FrameCount;
        var time = (float)frameCount;
        var verticalInitialVelocity = (catchPos.Y - throwPos.Y + gravityDistancePerFrameSquared * time * time / 2) / time;
        var horizontalInitialVelocity = (catchPos.X - throwPos.X) / time;
        Gravity = gravityDistancePerFrameSquared;
        Time = frameCount;
        StartVelocity = new(x: horizontalInitialVelocity, y: (float)verticalInitialVelocity);
        StartPosition = throwPos;
    }


    public AxisRange YRange()
    {
        return AxisRange.FromValues(Zenith.Y, StartPosition.Y, EndPosition.Y);
    }

    public AxisRange XRange()
    {
        return AxisRange.FromValues(StartPosition.X, EndPosition.X);
    }

}
