using Juggling;
using Raylib_cs;
using System.Numerics;

public static class Program
{
    private static readonly Vector2 _screenDims = new(800, 600);
    const float secondsPerFrame = 0.5f;
    const float dt = 0.01f;
    const int ballSize = 10;
    static int dtMs => (int)(dt * 1000);
    const float gravitySeconds = 900;
    private static Dictionary<int, Color> _ballColors = new() {
            { 0, Color.Red},
            { 1, Color.Green},
            { 2, Color.Blue},
            { 3, Color.Yellow},
            { 4, Color.Purple},
            { 5, Color.Orange},
        };
    const float lo = -75;
    const float li = -25;
    const float ro = 75;
    const float ri = 25;
    private static Pattern _pattern = new()
    {
        Hands = new() {
            new(){
                Actions = [
                    HandAction.Throw(li,0,0),
                    HandAction.Catch(lo,0,1),
                    HandAction.Throw(li,0,1),
                    HandAction.Catch(lo,0,2),
                    HandAction.Throw(li,0,2),
                    HandAction.Catch(lo,0,0),
                ]
            },
            new(){
                Actions = [
                    HandAction.Catch(ro,0,2),
                    HandAction.Throw(ri,0,2),
                    HandAction.Catch(ro,0,0),
                    HandAction.Throw(ri,0,0),
                    HandAction.Catch(ro,0,1),
                    HandAction.Throw(ri,0,1),
                ]
            }
        }
    };
    private static Vector2 ToRaylibPos(Vector2 originalPos) => originalPos + _screenDims / 2;
    public static void Main()
    {
        var throws = _pattern.GenerateThrows().ToList();
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
                Raylib.DrawCircle((int)ballPos.X, (int)ballPos.Y, ballSize, _ballColors[ballThrow.Ball]);
            }
            Raylib.DrawText(
                $"timeInFrames: {timeInFrames:F2}",
                10,        // x
                10,        // y
                20,        // font size
                Color.White
            );
            Raylib.EndDrawing();
            Thread.Sleep(dtMs);
        }
        Raylib.CloseWindow();
    }
}


