namespace NATSInternal.Services.Validations.Validators.Treatment;

public class TreatmentUpsertValidator : Validator<TreatmentUpsertRequestDto>
{
    public TreatmentUpsertValidator()
    {
        RuleFor(dto => dto.PaidDateTime)
            .IsValidStatsDateTime()
            .WithName(DisplayNames.PaidDateTime);
        RuleFor(dto => dto.ServiceAmount)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.ServiceAmount);
        RuleFor(dto => dto.ServiceVatFactor)
            .GreaterThanOrEqualTo(0)
            .WithName(DisplayNames.VatFactor);
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.CustomerId)
            .NotEmpty()
            .WithName(DisplayNames.Customer);
        RuleFor(dto => dto.TherapistId)
            .NotEmpty()
            .WithName(DisplayNames.Therapist);
        RuleFor(dto => dto.UpdateReason)
            .MaximumLength(255)
            .WithName(DisplayNames.Reason);
        RuleFor(dto => dto.Items)
            .NotEmpty()
            .WithName(DisplayNames.TreatmentItem);
        RuleForEach(dto => dto.Items)
            .NotNull()
            .SetValidator(new TreatmentItemValidator())
            .WithName(DisplayNames.TreatmentItem);
        RuleForEach(dto => dto.Photos)
            .NotNull()
            .SetValidator(new TreatmentPhotoValidator());
    }
}
