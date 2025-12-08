using Juggling;
using Raylib_cs;
using System.Numerics;

namespace Visualizer2D;

public class PatternVisualization
{
    private readonly Pattern _pattern;
    private readonly IReadOnlyList<BallThrow> _throws;
    private readonly IReadOnlyList<IMotionSequence> _hands;
    private readonly Vector2 _screenDims;
    private readonly float _dtSeconds;
    private readonly float _secondsPerFrame;
    private readonly float _ballSizePixels;
    private readonly Vector2 _handSizePixels;
    private readonly Vector2 _worldToScreenOffset;
    private readonly float _scaleModifier;
    private const int _pixelBuffer = 25;
    private Dictionary<int, Color> _ballColors = new() {
            { 0, Color.Red},
            { 1, Color.Green},
            { 2, Color.Blue},
            { 3, Color.Yellow},
            { 4, Color.Purple},
            { 5, Color.Orange},
            { 6, Color.White},
            { 7, Color.Beige},
        };
    public PatternVisualization(Pattern pattern, Vector2? screenDims = null, float dtSeconds = 0.01f, float secondsPerFrame = 0.5f, int ballSize = 11, Vector2? handSize = null, float gravityDistancePerFrameSquared = -900)
    {
        handSize ??= new(18, 3);
        _pattern = pattern;

        var gravityDistancePerFramesSquared = gravityDistancePerFrameSquared * secondsPerFrame * secondsPerFrame;
        pattern.PopulateAllMotion(gravityDistancePerFramesSquared);
        var throws = pattern.PopulatedThrows.ToList();
        _throws = throws;
        _hands = pattern.GetHandMotions(gravityDistancePerFramesSquared);
        _screenDims = screenDims ?? new(1000, 1000);
        _dtSeconds = dtSeconds;
        _secondsPerFrame = secondsPerFrame;

        ComputeScreenTransformation(throws.Select(t => t.PopulatedSolution!), _screenDims, ballSize, out var scaleModifier, out var offset);
        _scaleModifier = scaleModifier;

        _ballSizePixels = ballSize * scaleModifier;
        _handSizePixels = handSize.Value * scaleModifier;
        _worldToScreenOffset = offset;
    }
    private static (AxisRange X, AxisRange Y) GenerateBoundingRectangle(IEnumerable<ThrowSolution> throws)
    {
        var throwXs = throws.SelectMany(t => t.XRange().Extremes());
        var throwYs = throws.SelectMany(t => t.YRange().Extremes());
        return (new(throwXs), new(throwYs));
    }
    private static void ComputeScreenTransformation(
        IEnumerable<ThrowSolution> throws,
        Vector2 screenDims,
        float ballRadiusWorld,
        out float scaleModifier,
        out Vector2 worldToScreenOffset)
    {
        var (xRange, yRange) = GenerateBoundingRectangle(throws);

        // Expand world extents by the ball radius on all sides
        var expandedWidthX = xRange.Width + 2 * ballRadiusWorld;
        var expandedWidthY = yRange.Width + 2 * ballRadiusWorld;

        // Fixed pixel padding on each side
        var paddingPixels = _pixelBuffer;
        var innerDims = screenDims - new Vector2(2 * paddingPixels, 2 * paddingPixels);

        scaleModifier = Math.Min(
            innerDims.X / expandedWidthX,
            innerDims.Y / expandedWidthY
        );

        var screenCenter = screenDims / 2;
        var centroid = new Vector2(xRange.Center, yRange.Center) * scaleModifier;
        worldToScreenOffset = screenCenter + centroid;
    }
    private Vector2 ToRaylibPos(Vector2 originalPos)
    => -originalPos * _scaleModifier + _worldToScreenOffset;

    private Color GetBallColor(int n)
    {
        if (!_ballColors.TryGetValue(n, out var color))
        {
            var random = new Random();
            byte RandomByte() => (byte)random.Next(256);
            color = new(RandomByte(), RandomByte(), RandomByte());
            _ballColors[n] = color;
        }
        return color;

    }
    public void Display()
    {
        var dt = _dtSeconds;
        var timeSeconds = 0f;
        Raylib.InitWindow((int)_screenDims.X, (int)_screenDims.Y, "Juggling");
        while (!Raylib.WindowShouldClose())
        {
            timeSeconds += dt;
            var timeInFrames = (timeSeconds / _secondsPerFrame) % _pattern.FrameCount;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            foreach (var ballThrow in _throws)
            {
                var throwLocalTimeFrames = ballThrow.GetLocalFrameIndex(timeInFrames);
                if (throwLocalTimeFrames is null) continue;
                var ballPos = ballThrow.PopulatedSolution!.GetPosition(throwLocalTimeFrames.Value);
                ballPos = ToRaylibPos(ballPos);
                Raylib.DrawCircle((int)ballPos.X, (int)ballPos.Y, _ballSizePixels, GetBallColor(ballThrow.Ball));
            }
            foreach (var hand in _hands)
            {
                var handPos = hand.GetPosition(timeInFrames);
                handPos = ToRaylibPos(handPos);
                Raylib.DrawRectangle((int)handPos.X, (int)handPos.Y, (int)_handSizePixels.X, (int)_handSizePixels.Y, Color.Beige);
            }
            Raylib.EndDrawing();
            Thread.Sleep((int)(dt * 1000));
        }
        Raylib.CloseWindow();
    }
}
