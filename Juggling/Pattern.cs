using System.Diagnostics.CodeAnalysis;

namespace Juggling;

public class Pattern
{
    public int Length
    {
        get
        {
            if (!Hands.Any()) return 0;
            NormalizeLength();
            return Hands.First().Actions.Length;
        }
    }
    public List<HandPattern> Hands { get; init; } = new();
    public void ResetLength(int length)
    {
        foreach (var hand in Hands) hand.ResetLength(length);
    }
    private void NormalizeLength()
    {
        var length = Hands.Max(h => h.Actions.Length);
        ResetLength(length);
    }
    public bool TryGetStateSequence([NotNullWhen(returnValue: true)] out List<RoughState>? states, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        PopulateIndices();
        if (!TryGetInitialState(out var initialState, out var error))
        {
            errorMessage = error;
            return false;
        }
        var states = new List<RoughState>

        foreach (var frameActions in Frames())
        {

        }
    }

    public IEnumerable<List<HandAction?>> Frames()
    {
        NormalizeLength();
        for (var i = 0; i < Length; i++)
        {
            var handActions = Hands.Select(h => h.Actions[i]).ToList();
            yield return handActions;
        }
    }

    private Dictionary<int, List<HandAction>> GetInvolvedBallActions(List<HandAction?> handActions)
    {
        return handActions
            .WhereNotNull()
            .Where(action => action.ActionType.RequiresSpecifiedBall())
            .Where(p => p.Ball is not null)
            .GroupBy(p => p.Ball!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PopulateIndices()
    {
        foreach (var (idx, hand) in Hands.Index())
        {
            hand.HandIndex = idx;
            hand.PopulateFrameIndices();
        }
    }


    private bool TryGetInitialState([NotNullWhen(returnValue: true)] out RoughState? state, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        PopulateIndices();
        var ballsLeftToFind = AllBalls();
        var ballLocations = new Dictionary<int, int?>();
        foreach (var (idx, handActions) in Frames().Index())
        {
            var ballActionsThisFrame = GetInvolvedBallActions(handActions);
            foreach (var (ball, actions) in ballActionsThisFrame)
            {
                var positionCount = actions.Count;
                if (positionCount != 1)
                {
                    errorMessage = $"Ball {ball} appeared in {actions.Count} different hands on frame {idx}";
                    state = null;
                    return false;
                }
            }
            foreach (var ballFound in ballActionsThisFrame.Keys.Intersect(ballsLeftToFind))
            {
                ballsLeftToFind.Remove(ballFound);
                var action = ballActionsThisFrame[ballFound].Single();
                ballLocations[ballFound] = action.ActionType switch
                {
                    HandActionType.Throw => action.HandIndex, // Ball was in the hand
                    HandActionType.Catch => null, // Ball was in the air
                    _ => throw new InvalidOperationException($"The first time the balled appeared in the pattern it was with {nameof(HandActionType)} {action.ActionType}! Allowed actions: {HandActionType.Catch} and {HandActionType.Throw}")
                };
            }
            if (!ballsLeftToFind.Any()) break;
        }
        if (ballsLeftToFind.Any())
        {
            errorMessage = $"Balls {ballsLeftToFind.ToCollectionString()} were included in the pattern but never caught or thrown";
            state = null;
            return false;
        }
        state = new() { BallStates = ballLocations };
        errorMessage = null;
        return true;
    }

    private HashSet<int> AllBalls()
    {
        return Hands.SelectMany(h => h.Actions.Select(a => a.Ball).Where(b => b is not null).Select(b => b!.Value)).ToHashSet();
    }
}
