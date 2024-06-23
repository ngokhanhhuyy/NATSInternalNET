namespace NATSInternal.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToApplicationTime(this DateTime value)
    {
        return value.AddHours(7);
    }
}
