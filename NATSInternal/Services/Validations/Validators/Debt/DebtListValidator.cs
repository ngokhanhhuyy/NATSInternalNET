namespace NATSInternal.Services.Validations.Validators;

public class DebtListValidator : Validator<DebtListRequestDto>
{
    public DebtListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .IsEnumName(typeof(DebtListRequestDto.FieldOptions))
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.RangeTo)
            .GreaterThanOrEqualTo(dto => dto.RangeFrom)
            .When(dto => dto.RangeFrom.HasValue && dto.RangeTo.HasValue)
            .WithMessage(dto => ErrorMessages.LaterThanOrEqual
                .ReplaceComparisonValue(dto.RangeFrom?.ToVietnameseString()))
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