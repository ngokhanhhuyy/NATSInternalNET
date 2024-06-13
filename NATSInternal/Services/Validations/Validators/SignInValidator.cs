namespace NATSInternal.Services.Validations.Validators;

public class SignInValidator : Validator<SignInRequestDto>
{
    public SignInValidator()
    {
        RuleFor(dto => dto.UserName)
            .NotEmpty()
            .WithName(dto => DisplayNames.Get(nameof(dto.UserName)));
        RuleFor(dto => dto.Password)
            .NotEmpty()
            .WithName(dto => DisplayNames.Get(nameof(dto.Password)));
    }
}