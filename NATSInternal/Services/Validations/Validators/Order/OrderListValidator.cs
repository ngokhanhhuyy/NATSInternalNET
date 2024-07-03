namespace NATSInternal.Services.Validations.Validators;

public class OrderListValidator : Validator<OrderListRequestDto>
{
    public OrderListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .Must(IsEnumElementName<OrderListRequestDto.FieldOptions>)
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.RangeFrom)
            .LessThanOrEqualTo(dto => dto.RangeTo)
            .When(dto => dto.RangeFrom.HasValue && dto.RangeTo.HasValue)
            .WithName(DisplayNames.RangeFrom);
        RuleFor(dto => dto.RangeTo)
            .GreaterThanOrEqualTo(dto => dto.RangeFrom)
            .When(dto => dto.RangeTo.HasValue && dto.RangeFrom.HasValue)
            .WithName(DisplayNames.RangeTo);
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}
