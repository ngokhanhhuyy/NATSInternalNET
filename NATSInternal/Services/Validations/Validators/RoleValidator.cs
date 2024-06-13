namespace NATSInternal.Services.Validations.Validators;

public class RoleValidator : Validator<RoleRequestDto>
{
    public RoleValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithName(dto => DisplayNames.Get(nameof(dto.Name)));
    }
}
