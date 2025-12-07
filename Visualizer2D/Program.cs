using Juggling;
using Raylib_cs;
using System.Numerics;


Raylib.InitWindow(800, 600, "Juggling");
var ballColors = new Dictionary<int, Color>() {
    { 0, Color.Red},
    { 0, Color.Green},
    { 0, Color.Blue},
    { 0, Color.Yellow},
    { 0, Color.Purple},
    { 0, Color.Orange},
};
var pattern = new Pattern()
{
    Hands = new() {
        new(){
            Actions = {
                null,
                new(){ }
            }
        }
    }
};
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
