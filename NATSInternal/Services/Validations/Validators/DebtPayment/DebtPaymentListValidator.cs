namespace NATSInternal.Services.Validations.Validators;

public class DebtPaymentListValidator : Validator<DebtPaymentListRequestDto>
{
    public DebtPaymentListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .IsEnumName(typeof(DebtListRequestDto.FieldOptions))
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.Month)
            .IsValidQueryStatsMonth()
            .WithName(DisplayNames.Month);
        RuleFor(dto => dto.Year)
            .IsValidQueryStatsYear()
            .WithName(DisplayNames.Year);
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}