namespace NATSInternal.Services.Validations.Validators;

public class TreamentListValidator : AbstractValidator<TreatmentListRequestDto>
{
    public TreamentListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotNull()
            .IsEnumName(typeof(TreatmentListRequestDto.FieldOptions))
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.RangeFrom)
            .LessThanOrEqualTo(dto => dto.RangeTo)
            .WithMessage(GetRangeFromErrorMessage)
            .When(dto => dto.RangeFrom.HasValue && dto.RangeTo.HasValue)
            .WithName(DisplayNames.RangeFrom);
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50)
            .WithName(DisplayNames.ResultsPerPage);
    }

    private string GetRangeFromErrorMessage(TreatmentListRequestDto requestDto)
    {
        return ErrorMessages.EarlierThanOrEqual
            .ReplaceComparisonValue(requestDto.RangeTo.Value.ToVietnameseString());
    }
}
