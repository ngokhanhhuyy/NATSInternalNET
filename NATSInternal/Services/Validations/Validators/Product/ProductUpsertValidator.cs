namespace NATSInternal.Services.Validations.Validators.Product;

public class ProductUpsertValidator : Validator<ProductUpsertRequestDto>
{
    public ProductUpsertValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithName(DisplayNames.Name);
        RuleFor(dto => dto.Description)
            .MaximumLength(1000)
            .WithName(DisplayNames.Description);
        RuleFor(dto => dto.Unit)
            .NotEmpty()
            .MaximumLength(12)
            .WithName(DisplayNames.Unit);
        RuleFor(dto => dto.Price)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.Price);
        RuleFor(dto => dto.VatFactor)
            .GreaterThanOrEqualTo(0.0m)
            .WithName(DisplayNames.VatFactor);
        RuleFor(dto => dto.ThumbnailFile)
            .Must(IsValidImage)
            .When(dto => dto.ThumbnailFile != null)
            .WithName(DisplayNames.ThumbnailFile);
    }
}
