namespace NATSInternal.Services.Validations.Validators;

public class ConsultantUpsertValidator : Validator<ConsultantUpsertRequestDto>
{
    public ConsultantUpsertValidator()
    {
        RuleFor(dto => dto.PaidDateTime)
            .IsValidStatsDateTime()
            .WithName(DisplayNames.PaidDateTime);
        RuleFor(dto => dto.Amount)
            .GreaterThan(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.CustomerId)
            .NotEmpty()
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.Customer);

        RuleSet("Create", () => { });
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.UpdateReason)
                .NotEmpty()
                .MaximumLength(255)
                .WithName(DisplayNames.Reason);
        });
    }
}