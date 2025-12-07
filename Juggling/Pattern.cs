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
            states = null;
            return false;
        }
        states = new List<RoughState>() { initialState };

        var allBalls = AllBalls();
        foreach (var frameActions in Frames())
        {
            var ballStates = new Dictionary<int, int?>();
            var ballMultiActionsThisFrame = GetInvolvedBallActions(frameActions);
            if (!TryValidateBallActions(ballMultiActionsThisFrame, out var ballActionsThisFrame, out errorMessage))
            {
                states = null;
                return false;
            }
            foreach (var ball in allBalls)
            {
                var lastState = states.Last();
                if (!ballActionsThisFrame.TryGetValue(ball, out var ballAction))
                {
                    ballStates[ball] = lastState.BallStates[ball];
                    continue;
                }
                if (!TryGetBallNextRoughState(ball, ballAction, lastState, out var nextBallState, out errorMessage))
                {
                    states = null;
                    return false;
                }
                ballStates[ball] = nextBallState;
            }
            states.Add(new() { BallStates = ballStates });
        }

        // Finally, we need to make sure the ending state matches the initial state
        if (states.Last() != states.First())
        {
            errorMessage = "Ending state did not equal the starting state";
            return false;
        }
        errorMessage = null;
        return true;
    }

    private bool TryGetBallNextRoughState(int ball, HandAction action, RoughState lastState, out int? newState, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        if (action.Ball != ball)
        {
            errorMessage = SoftwareBug.Msg($"Ball mismatch - modifying ball {ball} with action on ball {action.Ball}");
            newState = null;
            return false;
        }
        var actionType = action.ActionType;
        if (!lastState.BallStates.TryGetValue(ball, out var ballLastState))
        {
            errorMessage = SoftwareBug.Msg($"Ball {ball} was not included in the carried over state! Make sure to initialize the state.");
            newState = null;
            return false;
        }
        if (actionType == HandActionType.Catch)
        {
            if (ballLastState is not null)
            {
                errorMessage = $"Ball {ball} already in hand {ballLastState} when it is caught by {action.HandIndex} on frame {action.FrameIndex}";
                newState = null;
                return false;
            }
            errorMessage = null;
            newState = action.HandIndex;
            return true;
        }
        if (actionType == HandActionType.Throw)
        {
            if (ballLastState is null)
            {
                errorMessage = $"Ball {ball} already in the air when it is thrown by {action.HandIndex} on frame {action.FrameIndex}";
                newState = null;
                return false;
            }
            errorMessage = null;
            newState = action.HandIndex;
            return true;
        }
        errorMessage = SoftwareBug.Msg($"Invalid action type {actionType}");
        newState = null;
        return false;
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
            .Where(action => action.ActionType.IsCatchOrThrow())
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

    private bool TryValidateBallActions(Dictionary<int, List<HandAction>> ballActionsThisFrame, [NotNullWhen(returnValue: true)] out Dictionary<int, HandAction>? ballActions, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        var singleActions = new Dictionary<int, HandAction>();
        foreach (var (ball, actions) in ballActionsThisFrame)
        {
            if (!actions.TryGetSingle(out var action))
            {
                errorMessage = $"Ball {ball} appeared in {actions.Count} different hands on frame {actions.First().FrameIndex}";
                ballActions = null;
                return false;
            }
            singleActions[ball] = action;
        }
        errorMessage = null;
        ballActions = singleActions;
        return true;
    }


    private bool TryGetInitialState([NotNullWhen(returnValue: true)] out RoughState? state, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        PopulateIndices();
        var ballsLeftToFind = AllBalls();
        var ballLocations = new Dictionary<int, int?>();
        foreach (var (idx, handActions) in Frames().Index())
        {
            var ballMultiActionsThisFrame = GetInvolvedBallActions(handActions);
            if (!TryValidateBallActions(ballMultiActionsThisFrame, out var ballActionsThisFrame, out errorMessage))
            {
                state = null;
                return false;
            }
            foreach (var ballFound in ballActionsThisFrame.Keys.Intersect(ballsLeftToFind))
            {
                ballsLeftToFind.Remove(ballFound);
                var action = ballActionsThisFrame[ballFound];
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
        return Hands.SelectMany(h => h.Actions.WhereNotNull().Select(a => a.Ball).WhereNotNull()).ToHashSet();
    }
}
