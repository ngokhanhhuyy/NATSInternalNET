namespace NATSInternal.Services.Validations.Validators;

public class OrderItemValidator : Validator<OrderItemRequestDto>
{
    public OrderItemValidator()
    {
        RuleFor(dto => dto.Amount)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.Amount);
        RuleFor(dto => dto.VatFactor)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.VatFactor);
        RuleFor(dto => dto.Quantity)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Quatity);
    }
}
