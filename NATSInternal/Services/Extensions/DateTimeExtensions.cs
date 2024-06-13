namespace NATSInternal.Services.Extensions;

public static class DateTimeExtensions
{
    private static string ConvertSecondNumberToText(double secondNumber, bool withSign = false)
    {
        string text = string.Empty;
        // Less than a minute, show "Just now"
        if (secondNumber < TimeSpan.FromMinutes(1).TotalSeconds)
            text = "Vừa xong";
        // Less than an hour, show "minutes"
        else if (secondNumber < TimeSpan.FromHours(1).TotalSeconds)
            text = ((int)Math.Round((double)secondNumber / 60)).ToString() + " phút";
        // Less than a day, show "hours"
        else if (secondNumber < TimeSpan.FromDays(1).TotalSeconds)
            text = ((int)Math.Round((double)secondNumber / (60 * 60))).ToString() + " giờ";
        // Less than a month (30 days), show "days"
        else if (secondNumber < TimeSpan.FromDays(30).TotalSeconds)
            text = ((int)Math.Round((double)secondNumber / (60 * 60 * 24))).ToString() + " ngày";
        // Less than a year, show "months"
        else if (secondNumber < TimeSpan.FromDays(365).TotalSeconds)
            text = ((int)Math.Round((double)secondNumber / (60 * 60 * 24 * 30))).ToString() + " tháng";
        // More than a year, show "years"
        else
            text = ((int)Math.Round((double)secondNumber / (60 * 60 * 24 * 365))).ToString() + " năm";

        // Adding sign
        if (withSign && secondNumber < 0) return $"- {text}";

        return text;
    }

    public static string DeltaTextFromDateTime(this DateTime dateTime, DateTime pastDateTime)
    {
        if (pastDateTime > dateTime)
            throw new ArgumentException(
                "Value for pastDateTime parameter cannot be later " +
                "than current DateTime value."
            );
        int secondsDifference = (dateTime - pastDateTime).Seconds;
        string deltaText = ConvertSecondNumberToText(secondsDifference);
        if (deltaText != "Vừa xong") return deltaText + " trước";

        return deltaText;
    }

    public static string DeltaTextUntilDateTime(this DateTime dateTime, DateTime futureDateTime)
    {
        if (futureDateTime < dateTime)
        {
            throw new ArgumentException(
                "Value for futureDateTime parameter cannot be earlier" +
                "than current DateTime value."
            );
        }

        int secondsDifference = (futureDateTime - dateTime).Seconds;
        string deltaText = ConvertSecondNumberToText(secondsDifference);
        if (deltaText != "Vừa xong") return deltaText + " nữa";

        return deltaText;
    }

    public static double YearDifferenceFromDateTime(this DateTime dateTime, DateTime pastDateTime)
    {
        if (pastDateTime > dateTime)
            throw new ArgumentException(
                "Value for pastDateTime parameter cannot be later " +
                "than current DateTime value."
            );

        double daysDifference = (dateTime - pastDateTime).TotalDays;
        return (double)Math.Round(daysDifference / 365.25, 1);
    }
}