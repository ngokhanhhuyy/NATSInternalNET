namespace NATSInternal.Services.Validations.Validators;

public class CustomerUpsertValidator : Validator<CustomerUpsertRequestDto> {
    public CustomerUpsertValidator() {
        RuleFor(dto => dto.FirstName)
            .NotNull()
            .MaximumLength(10)
            .WithName(dto => DisplayNames.Get(nameof(dto.FirstName)));
        RuleFor(dto => dto.MiddleName)
            .MaximumLength(20)
            .WithName(dto => DisplayNames.Get(nameof(dto.MiddleName)));
        RuleFor(dto => dto.LastName)
            .NotNull()
            .MaximumLength(10)
            .WithName(dto => DisplayNames.Get(nameof(dto.LastName)));
        RuleFor(dto => dto.NickName)
            .MaximumLength(35)
            .WithName(dto => DisplayNames.Get(nameof(dto.NickName)));
        RuleFor(dto => dto.Gender)
            .IsInEnum().WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.Gender)));
        RuleFor(dto => dto.Birthday)
            .Must(EqualOrEarlierThanToday)
            .WithName(dto => DisplayNames.Get(nameof(dto.Birthday)));
        RuleFor(dto => dto.PhoneNumber)
            .MaximumLength(15)
            .Matches(@"^[0-9]*$").WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.PhoneNumber)));
        RuleFor(dto => dto.ZaloNumber)
            .MaximumLength(15)
            .Matches(@"^[0-9]*$").WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.ZaloNumber)));
        RuleFor(dto => dto.FacebookUrl)
            .Must(IsValidFacebookUrl)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.FacebookUrl)));
        RuleFor(dto => dto.Email)
            .EmailAddress()
            .WithName(dto => DisplayNames.Get(nameof(dto.Email)));
        RuleFor(dto => dto.Address)
            .MaximumLength(255)
            .WithName(dto => DisplayNames.Get(nameof(dto.Address)));
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(dto => DisplayNames.Get(nameof(dto.Note)));
    }

    protected virtual bool IsValidFacebookUrl(string url) {
        if (url == null)
        {
            return true;
        }
        return url.StartsWith("https://facebook.com/");
    }
}