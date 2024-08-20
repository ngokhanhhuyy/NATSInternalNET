namespace NATSInternal.Services.Validations.Validators;

public class ExpenseListValidator : Validator<ExpenseListRequestDto>
{
    public ExpenseListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .Must(IsEnumElementName<ExpenseListRequestDto.FieldOptions>)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(DisplayNames.OrderByField);
        RuleFor(dto => dto.Month)
            .IsValidQueryStatsMonth()
            .WithName(DisplayNames.Month);
        RuleFor(dto => dto.Year)
            .IsValidQueryStatsYear()
            .WithName(DisplayNames.Year);
        RuleFor(dto => dto.Category)
            .IsInEnum().WithMessage(ErrorMessages.Invalid)
            .When(dto => dto.Category.HasValue)
            .WithName(DisplayNames.Category);
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(15)
            .WithName(DisplayNames.ResultsPerPage);
    }
}