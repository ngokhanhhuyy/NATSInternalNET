namespace NATSInternal.Services.Validations.Validators;

public class DebtPaymentUpsertValidator : Validator<DebtPaymentUpsertRequestDto>
{
    public DebtPaymentUpsertValidator(IStatsService statsService)
    {
        RuleFor(dto => dto.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.PaidDateTime)
            .LaterThanOrEqualToDateTime(statsService.GetResourceMinimumOpenedDateTime())
            .EarlierThanOrEqualToNow()
            .WithName(DisplayNames.PaidDateTime);
    }
}