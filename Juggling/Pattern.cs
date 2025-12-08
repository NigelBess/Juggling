using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Juggling;

public class Pattern
{
    /// <summary>
    /// Number of frames in the pattern
    /// </summary>
    public int FrameCount
    {
        get
        {
            if (!Hands.Any()) return 0;
            NormalizeLength();
            return Hands.First().Actions.Length;
        }
    }
    /// <summary>
    /// Action pattern for each hand (expected to all be length <see cref="FrameCount"/>).
    /// </summary>
    public List<HandPattern> Hands { get; init; } = new();

    /// <summary>
    /// All actions across all hands
    /// </summary>
    private IEnumerable<HandAction> AllActions => Hands.SelectMany(h => h.Actions.WhereNotNull());

    public IEnumerable<BallThrow> PopulatedThrows => AllActions.Select(a => a.PopulatedThrow).WhereNotNull();

    /// <summary>
    /// Changes length of the pattern to the specified length.
    /// </summary>
    /// <param name="length"></param>
    public void ResetLength(int length)
    {
        foreach (var hand in Hands) hand.ResetLength(length);
    }
    /// <summary>
    /// Forces all <see cref="Hands"/> to have the same length.
    /// </summary>
    private void NormalizeLength()
    {
        var length = Hands.Max(h => h.Actions.Length);
        ResetLength(length);
    }
    /// <summary>
    /// Builds the complete state sequence for the pattern (if possible)
    /// </summary>
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
            if (!TryGetBallActions(frameActions, out var ballActionsThisFrame, out errorMessage))
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
        if (!states.Last().Equals(states.First()))
        {
            errorMessage = $"Ending state did not equal the starting state: initial: {states.First()}. ending: {states.Last()}";
            return false;
        }
        errorMessage = null;
        return true;
    }

    /// <summary>
    /// Given some ball and some action on that ball, what is its new state?
    /// </summary>
    /// <param name="ball">The ball being acted upon</param>
    /// <param name="action">The action being applied to the ball</param>
    /// <param name="lastState">The last rough state of the system</param>
    /// <param name="newState">The new ball rough state (hand index or null if in the air)</param>
    /// <param name="errorMessage">Reason why we were unable to get the ball's next state (if applicable)</param>
    /// <returns><see langword="true"/> if the ball's next state was able to be determined</returns>
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
            newState = null;
            return true;
        }
        errorMessage = SoftwareBug.Msg($"Invalid action type {actionType}");
        newState = null;
        return false;
    }

    /// <summary>
    /// Enumerates the hand actions by frame. Each frame contains the actions for all hands.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<List<HandAction?>> Frames()
    {
        NormalizeLength();
        for (var i = 0; i < FrameCount; i++)
        {
            var handActions = Hands.Select(h => h.Actions[i]).ToList();
            yield return handActions;
        }
    }

    /// <summary>
    /// Given a frame, get all the actions relevant to each ball.
    /// </summary>
    /// <param name="handActions">A frame. see the comment for <see cref="Frames()"/>.</param>
    /// <returns>Each key of the dictionary is a ball index. The value is the list of actions from this frame that apply to the keyed ball.</returns>
    private Dictionary<int, List<HandAction>> GetInvolvedBallActions(List<HandAction?> handActions)
    {
        return handActions
            .WhereNotNull()
            .Where(action => action.ActionType.IsCatchOrThrow())
            .Where(p => p.Ball is not null)
            .GroupBy(p => p.Ball!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Populates all indices (hand indices, frame indices, etc) for this object and its children.
    /// </summary>
    public void PopulateIndices()
    {
        foreach (var (idx, hand) in Hands.Index())
        {
            hand.HandIndex = idx;
            hand.PopulateFrameIndices();
        }
    }

    /// <summary>
    /// Given some frame, attempts the get all single actions per ball.
    /// </summary>
    /// <param name="frame">Current frame</param>
    /// <param name="ballActions">Single action to each ball that had an action this frame</param>
    /// <param name="errorMessage">Reason why we were unable to get the ball actions</param>
    /// <returns><see langword="true"/> if we were able to get the ball actions.</returns>
    private bool TryGetBallActions(List<HandAction?> frame, [NotNullWhen(returnValue: true)] out Dictionary<int, HandAction>? ballActions, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        var singleActions = new Dictionary<int, HandAction>();
        var ballActionsThisFrame = GetInvolvedBallActions(frame);
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

    /// <summary>
    /// Attempts to get the single action applicable to a ball on a given frame
    /// </summary>
    /// <param name="frame">Current frame</param>
    /// <param name="ball">Ball we are interested in</param>
    /// <param name="ballAction">Action being applied to this ball (if there is one)</param>
    /// <returns><see langword="true"/> if there was an action this frame relevant to the given ball.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private bool TryGetBallAction(List<HandAction?> frame, int ball, [NotNullWhen(returnValue: true)] out HandAction? ballAction)
    {
        if (!TryGetBallActions(frame, out var ballActionsThisFrame, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }
        return ballActionsThisFrame.TryGetValue(ball, out ballAction);
    }

    private bool TryGetInitialState([NotNullWhen(returnValue: true)] out RoughState? state, [NotNullWhen(returnValue: false)] out string? errorMessage)
    {
        PopulateIndices();
        var ballsLeftToFind = AllBalls();
        var ballLocations = new Dictionary<int, int?>();
        foreach (var (idx, frameActions) in Frames().Index())
        {
            if (!TryGetBallActions(frameActions, out var ballActionsThisFrame, out errorMessage))
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

    /// <summary>
    /// All balls involved in the pattern.
    /// </summary>
    /// <returns></returns>
    private HashSet<int> AllBalls()
    {
        return Hands.SelectMany(h => h.Actions.WhereNotNull().Select(a => a.Ball).WhereNotNull()).ToHashSet();
    }

    /// <summary>
    /// The throws involved in the pattern
    /// </summary>
    public IEnumerable<BallThrow> GenerateThrows()
    {
        PopulateIndices();
        if (!TryGetStateSequence(out _, out var error)) throw new InvalidOperationException(error);
        var throwActions = AllActions.Where(a => a.ActionType == HandActionType.Throw);
        var frames = Frames().ToList();
        var frameCount = FrameCount;
        foreach (var throwAction in throwActions)
        {
            for (var i = 1; i < frameCount; i++)
            {
                var frame = frames[(throwAction.FrameIndex + i) % frameCount];
                var ball = throwAction.Ball!.Value;
                if (!TryGetBallAction(frame, throwAction.Ball!.Value, out var catchAction))
                {
                    continue;
                }
                if (catchAction.ActionType != HandActionType.Catch) throw new InvalidOperationException($"Attempting to {catchAction.ActionType} a ball {ball} using hand {catchAction.HandIndex} on frame {catchAction.FrameIndex} but the ball is in the air.");
                yield return new()
                {
                    Catch = catchAction,
                    Throw = throwAction,
                    LoopFrameCount = frameCount,
                };
                break;
            }
        }
    }

    /// <summary>
    /// Populates all motion parameters, including throws, ball motion, and hand motion.
    /// </summary>
    public void PopulateAllMotion(float gravityDistancePerFrameSquared)
    {
        foreach (var ballThrow in GenerateThrows())
        {
            ballThrow.PopulateSolution(gravityDistancePerFrameSquared);
            foreach (var action in ballThrow.Actions) action.PopulatedThrow = ballThrow;

        }

        foreach (var action in AllActions)
        {
            var populatedSolution = action.PopulatedThrow!.PopulatedSolution!;
            action.HandMotionEndpoint = action.ActionType switch
            {
                HandActionType.Move => new() { Position = action.Position, Velocity = Vector2.Zero },
                HandActionType.Throw => new() { Position = populatedSolution.StartPosition, Velocity = populatedSolution.StartVelocity },
                HandActionType.Catch => new() { Position = populatedSolution.EndPosition, Velocity = populatedSolution.EndVelocity },
                _ => throw new NotImplementedException($"Unable to determine motion endpoint for action type {action.ActionType}")
            };
        }
    }

    public List<IMotionSequence> GetHandMotions(float gravityDistancePerFrameSquared)
    {
        PopulateIndices();
        PopulateAllMotion(gravityDistancePerFrameSquared);
        return Hands.Select(h => h.GenerateMotionSequence()).ToList();
    }


    public static Pattern FromHands(params IEnumerable<HandAction?>[] hands)
    {
        return new()
        {
            Hands = hands.Select(h => HandPattern.FromActions(h)).ToList()
        };
    }


}
