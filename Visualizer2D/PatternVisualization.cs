using Juggling;
using Raylib_cs;
using System.Numerics;

namespace Visualizer2D;

public class PatternVisualization
{
    private readonly Pattern _pattern;
    private readonly IReadOnlyDictionary<BallThrow, ThrowSolution> _throwSolutions;
    private readonly Vector2 _screenDims;
    private readonly float _dtSeconds;
    private readonly float _secondsPerFrame;
    private readonly int _ballSize;
    private readonly Vector2 _worldToScreenOffset;
    private readonly Vector2 _scaleModifier;
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
    public PatternVisualization(Pattern pattern, Vector2? screenDims = null, float dtSeconds = 0.01f, float secondsPerFrame = 0.5f, int ballSize = 10, float gravityPixelPerSecondSquared = 900)
    {
        _pattern = pattern;
        var throws = pattern.GenerateThrows().ToList();
        var gravityPixelPerFramesSquared = gravityPixelPerSecondSquared * secondsPerFrame * secondsPerFrame;
        var throwSolutions = throws.ToDictionary(t => t, elementSelector: t => t.ComputeSolution(gravityPixelPerFramesSquared));
        _throwSolutions = throwSolutions;
        _screenDims = screenDims ?? new(600, 1000);
        _dtSeconds = dtSeconds;
        _secondsPerFrame = secondsPerFrame;
        _ballSize = ballSize;
        ComputeScreenTransformation(throwSolutions.Values, out var scaleModifier, out var offset);
        _scaleModifier = scaleModifier;
        _worldToScreenOffset = offset;
    }
    private (AxisRange X, AxisRange Y) GenerateBoundingRectangle(IEnumerable<ThrowSolution> throws)
    {
        var throwXs = throws.SelectMany(t => t.XRange().Extremes());
        var throwYs = throws.SelectMany(t => t.YRange().Extremes());
        return (new(throwXs), new(throwYs));
    }
    private void ComputeScreenTransformation(IEnumerable<ThrowSolution> throws, out Vector2 scaleModifier, out Vector2 worldToScreenOffset)
    {
        var (xRange, yRange) = GenerateBoundingRectangle(throws);
        var dims = _screenDims;
        var screenCenter = dims / 2;
        var centroid = new Vector2(xRange.Center(), yRange.Center());
        worldToScreenOffset = screenCenter - centroid;
        scaleModifier = new(dims.X / xRange.Width(), dims.Y / xRange.Width());
    }
    private Vector2 ToRaylibPos(Vector2 originalPos) => originalPos * _scaleModifier + _worldToScreenOffset;
    private Color GetBallColor(int n)
    {
        if (_ballColors.TryGetValue(n, out var color)) return color;
        var random = new Random();
        byte RandomByte() => (byte)random.Next(256);
        return Color.FromHSV(RandomByte(), 1, 1);
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

            foreach (var (ballThrow, solution) in _throwSolutions)
            {
                var throwLocalTimeFrames = ballThrow.GetLocalFrameIndex(timeInFrames);
                if (throwLocalTimeFrames is null) continue;
                var ballPos = solution.GetPosition(throwLocalTimeFrames.Value);
                ballPos = ToRaylibPos(ballPos);
                Raylib.DrawCircle((int)ballPos.X, (int)ballPos.Y, _ballSize, GetBallColor(ballThrow.Ball));
            }
            Raylib.EndDrawing();
            Thread.Sleep((int)(dt * 1000));
        }
        Raylib.CloseWindow();
    }
}
