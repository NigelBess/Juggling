namespace Juggling;

public class HandPattern
{
    public HandAction?[] Actions { get; set; } = [];
    public void ResetLength(int length)
    {
        var newActions = new HandAction?[length];
        Array.Copy(Actions, newActions, length);
        Actions = newActions;
    }
    private int? _handIndex;
    public int HandIndex
    {
        get
        {
            if (_handIndex is null) throw new InvalidOperationException("Hand index has not been populated");
            return _handIndex.Value;
        }
        set
        {
            foreach (var action in Actions)
            {
                action?.HandIndex = value;
            }
            _handIndex = value;
        }
    }
    public void PopulateFrameIndices()
    {
        foreach (var (idx, action) in Actions.Index())
        {
            if (action is null) continue;
            action.FrameIndex = idx;
        }
    }

    public HandMotionSequence GenerateMotionSequence()
    {
        var actions = Actions.WhereNotNull().ToList();
        var actionCount = actions.Count;
        var motions = new List<HandMotion>(actionCount);
        for (var i = 0; i < actionCount; i++)
        {
            var start = actions[i];
            var end = actions[(i + 1) % actionCount];
            motions.Add(new(start.HandMotionEndpoint!, end.HandMotionEndpoint!, actionCount));
        }
        return new HandMotionSequence(motions);
    }

    public static implicit operator HandPattern(HandAction?[] actions) =>
    new() { Actions = actions };
    public static HandPattern FromActions(IEnumerable<HandAction?> actions) => new() { Actions = actions.ToArray() };
}
