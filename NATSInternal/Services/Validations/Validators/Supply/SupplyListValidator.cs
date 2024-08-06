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
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Month)
            .When(dto => dto.Year.HasValue && dto.Year == DateTime.UtcNow.ToApplicationTime().Year)
            .LessThanOrEqualTo(12)
            .When(dto => dto.Year.HasValue && dto.Year < DateTime.UtcNow.ToApplicationTime().Year)
            .WithName(DisplayNames.Month);
        RuleFor(dto => dto.Year)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Year)
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
