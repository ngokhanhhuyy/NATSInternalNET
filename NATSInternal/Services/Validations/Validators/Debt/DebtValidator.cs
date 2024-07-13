namespace NATSInternal.Services.Validations.Validators;

public class DebtValidator : Validator<DebtUpsertRequestDto>
{
    public DebtValidator(IStatsService statsService)
    {
        RuleFor(dto => dto.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.CreatedDateTime)
            .LaterThanOrEqualToDateTime(statsService.GetResourceMinimumOpenedDateTime())
            .EarlierThanOrEqualToNow()
            .WithName(DisplayNames.CreatedDateTime);
        RuleFor(dto => dto.CustomerId)
            .NotEmpty()
            .WithName(DisplayNames.Customer);
    }
}