namespace NATSInternal.Services.Dtos;

public class MonthYearResponseDto
{
    public int Month { get; set; }
    public int Year { get; set; }

    public MonthYearResponseDto(int year, int month)
    {
        Month = month;
        Year = year;
    }
}
