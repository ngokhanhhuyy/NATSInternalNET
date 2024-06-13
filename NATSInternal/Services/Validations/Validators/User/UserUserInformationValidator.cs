namespace NATSInternal.Services.Validations.Validators;

public class UserUserInformationValidator : Validator<UserUserInformationRequestDto>
{
    public UserUserInformationValidator()
    {
        RuleFor(dto => dto.JoiningDate)
            .Must(EqualOrEarlierThanToday)
            .WithMessage(ErrorMessages.EarlierThanOrEqualToday
                .Replace("{Today}", DateOnly.FromDateTime(DateTime.Today).ToString("dd-MM-yyyy")))
            .WithName(dto => DisplayNames.Get(nameof(dto.JoiningDate)));
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(dto => DisplayNames.Get(nameof(dto.Note)));
        RuleFor(dto => dto.Role)
            .NotNull()
            .SetValidator(new RoleValidator());
    }
}
