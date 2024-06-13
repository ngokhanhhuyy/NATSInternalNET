namespace NATSInternal.Services.Validations.Validators;

public class UserPersonalInformationValidator : Validator<UserPersonalInformationRequestDto>
{
    public UserPersonalInformationValidator()
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty()
            .MaximumLength(15)
            .WithName(dto => DisplayNames.Get(nameof(dto.FirstName)));
        RuleFor(dto => dto.MiddleName)
            .MaximumLength(20)
            .WithName(dto => DisplayNames.Get(nameof(dto.MiddleName)));
        RuleFor(dto => dto.LastName)
            .NotEmpty()
            .MaximumLength(15)
            .WithName(dto => DisplayNames.Get(nameof(dto.LastName)));
        RuleFor(dto => dto.Birthday)
            .Must(EqualOrEarlierThanToday)
            .WithMessage(ErrorMessages.EarlierThanOrEqualToday
                .Replace("{Today}", DateOnly.FromDateTime(DateTime.Today).ToString("dd-MM-yyyy")))
            .WithName(dto => DisplayNames.Get(nameof(dto.Birthday)));
        RuleFor(dto => dto.PhoneNumber)
            .MaximumLength(12)
            .Matches(PhoneNumberRegex).WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.PhoneNumber)));
        RuleFor(dto => dto.Email)
            .EmailAddress()
            .WithName(dto => DisplayNames.Get(nameof(dto.Email)));
        RuleFor(dto => dto.AvatarFile)
            .Must(IsValidImage)
            .When(dto => dto.AvatarFile != null && dto.AvatarChanged)
            .WithName(dto => DisplayNames.Get(nameof(dto.AvatarFile)));
    }
}
