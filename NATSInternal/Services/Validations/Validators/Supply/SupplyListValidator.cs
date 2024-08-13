namespace NATSInternal.Services.Validations.Validators;

public class SupplyListValidator : Validator<SupplyListRequestDto>
{
    public SupplyListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .Must(IsEnumElementName<SupplyListRequestDto.FieldOptions>)
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
            .LessThan(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}
