using Raylib_cs;
using System.Numerics;


Raylib.InitWindow(800, 600, "Juggling");
while (!Raylib.WindowShouldClose())
{
    // update juggling state here

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    // Draw balls
    foreach (var ball in new Vector2[] { new(1, 2), new(300, 400) })
        Raylib.DrawCircle((int)ball.X, (int)ball.Y, 10, Color.Red);

    Raylib.EndDrawing();
}
Raylib.CloseWindow();
