namespace NATSInternal.Services.Validations.Validators;

public class UserCreateValidator : Validator<UserCreateRequestDto>
{
    public UserCreateValidator()
    {
        RuleFor(dto => dto.UserName)
            .NotEmpty()
            .MaximumLength(20)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage(ErrorMessages.InvalidUserNamePattern)
            .WithName(dto => DisplayNames.Get(nameof(dto.UserName)));
        RuleFor(dto => dto.Password)
            .NotEmpty()
            .Length(8, 20)
            .WithName(dto => DisplayNames.Get(nameof(dto.Password)));
        RuleFor(dto => dto.ConfirmationPassword)
            .NotEmpty()
            .Must(ConfirmationPasswordMatches)
            .WithMessage(dto => ErrorMessages.MismatchedWith
                .Replace("{ComparisonPropertyName}", DisplayNames.Get(nameof(dto.Password))))
            .WithName(dto => DisplayNames.Get(nameof(dto.ConfirmationPassword)));
        RuleFor(dto => dto.PersonalInformation)
            .NotNull()
            .SetValidator(new UserPersonalInformationValidator())
            .WithName(dto => DisplayNames.Get(nameof(dto.PersonalInformation)));
        RuleFor(dto => dto.UserInformation)
            .NotNull()
            .SetValidator(new UserUserInformationValidator())
            .WithName(dto => DisplayNames.Get(nameof(dto.UserInformation)));
    }

    protected virtual bool ConfirmationPasswordMatches(
            UserCreateRequestDto dto,
            string confirmationPassword)
    {
        return dto.Password == confirmationPassword;
    }
}
