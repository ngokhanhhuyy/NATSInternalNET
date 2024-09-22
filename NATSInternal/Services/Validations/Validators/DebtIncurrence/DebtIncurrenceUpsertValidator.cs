namespace NATSInternal.Services.Validations.Validators;

public class DebtIncurrenceUpsertValidator : Validator<DebtIncurrenceUpsertRequestDto>
{
    public DebtIncurrenceUpsertValidator()
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
        
        RuleSet("Create", () =>
        {
            RuleFor(dto => dto.CustomerId)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(DisplayNames.Customer);
        });
        
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.UpdatingReason)
                .NotEmpty()
                .MaximumLength(255)
                .WithName(DisplayNames.Reason);
        });
    }
}