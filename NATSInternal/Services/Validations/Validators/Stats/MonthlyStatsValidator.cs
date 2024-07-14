namespace NATSInternal.Services.Validations.Validators;

public class MonthlyStatsValidator : Validator<MonthlyStatsRequestDto>
{
    public MonthlyStatsValidator()
    {
        RuleFor(dto => dto.RecordedYear)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(DisplayNames.RecordedYear);
        RuleFor(dto => dto.RecordedMonth)
            .Must(IsValidMonth)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(DisplayNames.RecordedMonth);
    }

    private bool IsValidMonth(MonthlyStatsRequestDto requestDto, int recordedMonth)
    {
        if (recordedMonth < 0 || recordedMonth > 12)
        {
            return false;
        }

        if (requestDto.RecordedYear == DateTime.UtcNow.ToApplicationTime().Year)
        {
            if (recordedMonth > DateTime.UtcNow.ToApplicationTime().Month)
            {
                return false;
            }
        }

        return true;
    }
}