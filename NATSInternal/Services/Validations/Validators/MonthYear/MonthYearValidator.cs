namespace NATSInternal.Services.Validations.Validators;

public class MonthYearValidator : Validator<MonthYearRequestDto>
{
    public MonthYearValidator()
    {
        RuleFor(dto => dto.Month)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Month)
            .When(dto => dto.Year == DateTime.UtcNow.ToApplicationTime().Year)
            .LessThanOrEqualTo(12)
            .When(dto => dto.Year < DateTime.UtcNow.ToApplicationTime().Year)
            .WithName(DisplayNames.Month);
        RuleFor(dto => dto.Year)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Year)
            .WithName(DisplayNames.Year);
    }
}
