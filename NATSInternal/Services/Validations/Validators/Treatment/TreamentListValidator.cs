namespace NATSInternal.Services.Validations.Validators;

public class TreamentListValidator : Validator<TreatmentListRequestDto>
{
    public TreamentListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotNull()
            .IsEnumName(typeof(TreatmentListRequestDto.FieldOptions))
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
