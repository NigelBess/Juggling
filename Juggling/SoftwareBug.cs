namespace Juggling;

internal static class SoftwareBug
{
    public static string Msg(string msg)
    {
        try
        {
            throw new Exception(msg); // To get a stack strace
        }
        catch (Exception e)
        {
            return $"You found a software bug! Dev debug details: {msg}\n Stack Trace:\n{e.StackTrace}";
        }

    }
}
