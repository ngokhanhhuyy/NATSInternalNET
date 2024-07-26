namespace NATSInternal.Services.Validations.Validators;

public class TreatmentPhotoValidator : Validator<TreatmentPhotoRequestDto>
{
    public TreatmentPhotoValidator()
    {
        RuleFor(dto => dto.File)
            .IsValidImage()
            .When(dto => !dto.Id.HasValue)
            .WithName(DisplayNames.File);
    }
}
