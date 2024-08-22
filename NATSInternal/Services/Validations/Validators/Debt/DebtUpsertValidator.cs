namespace NATSInternal.Services.Validations.Validators;

public class DebtUpsertValidator : Validator<DebtIncurrenceUpsertRequestDto>
{
    public DebtUpsertValidator(IStatsService statsService)
    {
        RuleFor(dto => dto.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.IncurredDateTime)
            .IsValidStatsDateTime()
            .WithName(DisplayNames.IncurredDateTime);
        
        RuleSet("Create", () => { });
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.UpdatingReason)
                .NotEmpty()
                .MaximumLength(255)
                .WithName(DisplayNames.Reason);
        });
    }
}