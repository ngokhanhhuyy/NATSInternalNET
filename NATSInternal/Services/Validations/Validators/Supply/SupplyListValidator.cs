namespace NATSInternal.Services.Validations.Validators;

public class SupplyListValidator : Validator<SupplyListRequestDto>
{
    public SupplyListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .Must(IsEnumElementName<SupplyListRequestDto.FieldOptions>)
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.RangeFrom)
            .LessThan(dto => dto.RangeTo)
            .When(dto => dto.RangeFrom.HasValue && dto.RangeTo.HasValue)
            .WithName(DisplayNames.RangeFrom);
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThan(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}
