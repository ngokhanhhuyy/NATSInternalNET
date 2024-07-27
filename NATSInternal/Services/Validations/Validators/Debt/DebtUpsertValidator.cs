namespace NATSInternal.Services.Validations.Validators;

public class DebtUpsertValidator : Validator<DebtUpsertRequestDto>
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
        RuleFor(dto => dto.CustomerId)
            .NotEmpty()
            .WithName(DisplayNames.Customer);
        
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