namespace NATSInternal.Services.Validations.Validators;

public class ConsultantUpsertValidator : Validator<ConsultantUpsertRequestDto>
{
    public ConsultantUpsertValidator()
    {
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

        RuleSet("Create", () => {
            RuleFor(dto => dto.PaidDateTime)
                .GreaterThanOrEqualTo(GetMinimumPaidDateTime())
                .When(dto => dto.PaidDateTime.HasValue)
                .WithName(DisplayNames.PaidDateTime);
        });

        RuleSet("Update", () => { });
    }

    private static DateTime GetMinimumPaidDateTime()
    {
        return new DateTime(
            DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Year,
            DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Month,
            1, 0, 0, 0);
    }
}