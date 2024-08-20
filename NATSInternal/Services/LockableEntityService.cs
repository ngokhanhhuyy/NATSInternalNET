using Microsoft.EntityFrameworkCore;

namespace NATSInternal.Services;

public class LockableEntityService
{
    protected List<MonthYearResponseDto> GenerateMonthYearOptions(MonthYearResponseDto earliestRecordedMonthYear)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        int currentYear = currentDateTime.Year;
        int currentMonth = currentDateTime.Month;
        List<MonthYearResponseDto> monthYearOptions = new List<MonthYearResponseDto>();
        if (earliestRecordedMonthYear != null)
        {
            int initializingYear = earliestRecordedMonthYear.Year;
            while (initializingYear <= currentYear)
            {
                int initializingMonth = 1;
                if (initializingYear == earliestRecordedMonthYear.Year)
                {
                    initializingMonth = earliestRecordedMonthYear.Month;
                }
                while (initializingMonth <= 12)
                {
                    MonthYearResponseDto option;
                    option = new MonthYearResponseDto(initializingYear, initializingMonth);
                    monthYearOptions.Add(option);
                    initializingMonth += 1;
                    if (initializingYear == currentYear && initializingMonth > currentMonth)
                    {
                        break;
                    }
                }
                initializingYear += 1;
            }
            monthYearOptions.Reverse();
        }
        else
        {
            monthYearOptions.Add(new MonthYearResponseDto(currentYear, currentMonth));
        }

        return monthYearOptions;
    }
}
