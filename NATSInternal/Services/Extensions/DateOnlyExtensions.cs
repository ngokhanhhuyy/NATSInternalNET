namespace NATSInternal.Services.Extensions;

public static class DateOnlyExtensions
{
    public static string ToVietnameseString(this DateOnly date)
    {
        return $"{date.Day} tháng {date.Month}, {date.Year}";
    }
}