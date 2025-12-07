using Juggling;
using Visualizer2D;

public static class Program
{

    public static void Main()
    {
        var pattern = Patterns.Io(101);
        var visualization = new PatternVisualization(pattern);
        visualization.Display();
    }
}


