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
}
