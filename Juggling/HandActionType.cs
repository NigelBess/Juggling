namespace Juggling;

public enum HandActionType
{
    Undefined,
    Catch,
    Throw,
    Move,
}

public static class HandActionTypeExtensions
{
    public static bool IsCatchOrThrow(this HandActionType action) => action == HandActionType.Throw || action == HandActionType.Catch;
}