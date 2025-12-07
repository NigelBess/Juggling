using Juggling;
using Raylib_cs;
using System.Numerics;

public static class Program
{
    private static readonly Vector2 _screenDims = new(800, 600);
    const float secondsPerFrame = 1;
    const float dt = 0.01f;
    const int ballSize = 10;
    static int dtMs => (int)(dt * 1000);
    const float gravitySeconds = 90;
    private static Dictionary<int, Color> _ballColors = new() {
            { 0, Color.Red},
            { 0, Color.Green},
            { 0, Color.Blue},
            { 0, Color.Yellow},
            { 0, Color.Purple},
            { 0, Color.Orange},
        };
    private static Pattern _pattern = new()
    {
        Hands = new() {
            new(){
                Actions = [
                    null,
                    HandAction.Catch(0,0,1),
                    null,
                    HandAction.Throw(0,0,1),
                ]
            }
        }
    };
    private static Vector2 ToRaylibPos(Vector2 originalPos) => originalPos + _screenDims / 2;
    public static void Main()
    {
        var throws = _pattern.GenerateThrows();
        var gravityFrames = gravitySeconds * secondsPerFrame * secondsPerFrame;
        var throwSolutions = throws.ToDictionary(t => t, elementSelector: t => t.GenerateSolution(gravityFrames));
        Raylib.InitWindow((int)_screenDims.X, (int)_screenDims.Y, "Juggling");

        var loopTimeFrames = _pattern.FrameCount;
        var timeSeconds = 0f;
        while (!Raylib.WindowShouldClose())
        {
            timeSeconds += dt;
            var timeInFrames = (timeSeconds / secondsPerFrame) % loopTimeFrames;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            foreach (var ballThrow in throws)
            {
                var throwLocalTimeFrames = ballThrow.GetLocalFrameIndex(timeInFrames);
                if (throwLocalTimeFrames is null) continue;
                var ballPos = throwSolutions[ballThrow].GetPosition(throwLocalTimeFrames.Value);
                ballPos = ToRaylibPos(ballPos);
                var ballColor = _ballColors[ballThrow.Ball];
                Raylib.DrawCircle((int)ballPos.X, (int)ballPos.Y, ballSize, Color.Red);
            }

            Raylib.EndDrawing();
            Thread.Sleep(dtMs);
        }
        Raylib.CloseWindow();
    }
}


