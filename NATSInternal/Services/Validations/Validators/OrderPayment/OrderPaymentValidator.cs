namespace NATSInternal.Services.Validations;

public class OrderPaymentValidator : Validator<OrderPaymentRequestDto>
{
    public OrderPaymentValidator()
    {
        RuleFor(dto => dto.Amount)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleSet("Create", () => { });
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.PaidDateTime)
                .NotNull()
                .WithName(DisplayNames.PaidDateTime);
        });
    }
}
