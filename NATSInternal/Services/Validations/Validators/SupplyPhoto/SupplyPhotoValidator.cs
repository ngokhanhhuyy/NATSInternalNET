namespace NATSInternal.Services.Validations.Validators;

public class SupplyPhotoValidator : Validator<SupplyPhotoRequestDto>
{
    public SupplyPhotoValidator()
    {
        RuleSet("Create", () =>
        {
            RuleFor(dto => dto.File)
                .Must(IsValidImage).WithMessage(ErrorMessages.Invalid)
                .WithName(DisplayNames.File);
        });

        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.File)
                .NotNull()
                .When(dto => !dto.Id.HasValue)
                .Must(IsValidImage).WithName(ErrorMessages.Invalid)
                .When(dto => dto.File != null)
                .WithName(DisplayNames.File);
        });
    }
}
