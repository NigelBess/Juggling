namespace Juggling;

public class AxisRange
{
    public float Min { get; set; }
    public float Max { get; set; }
    public AxisRange(IEnumerable<float> values)
    {
        Min = values.Min();
        Max = values.Max();
    }
    public IEnumerable<float> Extremes() => [Min, Max];
    public float Center() => (Min + Max) / 2;
    public float Width() => Max - Min;

    public static AxisRange FromValues(params float[] values) => new(values);
}
