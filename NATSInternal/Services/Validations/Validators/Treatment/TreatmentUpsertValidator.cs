namespace NATSInternal.Services.Validations.Validators.Treatment;

public class TreatmentUpsertValidator : Validator<TreatmentUpsertRequestDto>
{
    public TreatmentUpsertValidator()
    {
        RuleFor(dto => dto.OrderedDateTime)
            .GreaterThanOrEqualTo(MinimumOrderedDateTime)
            .When(dto => dto.OrderedDateTime.HasValue)
            .WithMessage(dto => GetMinimumOrderedDateTimeErrorMessage(dto))
            .WithName(DisplayNames.OrderedDateTime);
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

    public DateTime MinimumOrderedDateTime
    {
        get
        {
            DateTime minDate = DateTime.UtcNow.ToApplicationTime().AddMonths(-1);
            return new DateTime(minDate.Year, minDate.Month, 1, 0, 0, 0);
        }
    }

    private string GetMinimumOrderedDateTimeErrorMessage(TreatmentUpsertRequestDto requestDto)
    {
        return ErrorMessages.LaterThanOrEqual
            .ReplacePropertyName(DisplayNames.OrderedDateTime)
            .ReplaceComparisonValue(MinimumOrderedDateTime.ToVietnameseString());
    }


}
