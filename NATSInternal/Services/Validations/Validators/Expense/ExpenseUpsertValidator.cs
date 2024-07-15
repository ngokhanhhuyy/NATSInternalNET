namespace NATSInternal.Services.Validations;

public class ExpenseUpsertValidator : Validator<ExpenseUpsertRequestDto>
{
    public ExpenseUpsertValidator()
    {
        RuleFor(dto => dto.Amount)
            .GreaterThan(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Category)
            .IsInEnum().WithMessage(ErrorMessages.Invalid)
            .WithName(DisplayNames.Category);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.PayeeName)
            .NotEmpty()
            .MaximumLength(100)
            .WithName(DisplayNames.PayeeName);

        RuleSet("Create", () => {
            RuleFor(dto => dto.PaidDateTime)
                .GreaterThanOrEqualTo(GetMinimumPaidDateTime())
                .When(dto => dto.PaidDateTime.HasValue)
                .WithName(DisplayNames.PaidDateTime);
            RuleForEach(dto => dto.Photos)
                .SetValidator(new ExpensePhotoValidator(), ruleSets: "Create");
        });

        RuleSet("Update", () => {
            RuleForEach(dto => dto.Photos)
                .SetValidator(new ExpensePhotoValidator(), ruleSets: "Update");
        });
    }

    private static DateTime GetMinimumPaidDateTime()
    {
        return new DateTime(
            DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Year,
            DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Month,
            1, 0, 0, 0);
    }
}
