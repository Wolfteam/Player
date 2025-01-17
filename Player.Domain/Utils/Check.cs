namespace Player.Domain.Utils;

public static class Check
{
    public static void NotNull<T>(T? val, string parameterName)
    {
        if (val == null)
        {
            throw new ArgumentNullException(parameterName, "The provided value cannot be null");
        }
    }

    public static void NotEmpty(string? val, string parameterName)
    {
        NotNull(val, parameterName);
        if (val!.Trim().Length == 0)
        {
            throw new ArgumentException("You need to provide a valid value", parameterName);
        }
    }

    public static void NotEmpty(long? val, string parameterName)
    {
        NotNull(val, parameterName);
        if (val <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, val, "You need to provide a valid value");
        }
    }

    public static void NotNull<T>(IReadOnlyCollection<T>? values, string parameterName)
    {
        if (values == null)
        {
            throw new ArgumentNullException(parameterName, "The provided value cannot be null");
        }
    }

    public static bool IsEmpty(long val)
        => val <= 0;
}