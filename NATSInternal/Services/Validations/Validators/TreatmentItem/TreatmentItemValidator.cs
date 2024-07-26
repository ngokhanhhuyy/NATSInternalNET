namespace NATSInternal.Services.Validations.Validators;

public class TreatmentItemValidator : Validator<TreatmentItemRequestDto>
{
    public TreatmentItemValidator()
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
        RuleFor(dto => dto.ProductId)
            .NotEmpty()
            .WithName(DisplayNames.Product);
    }
}
