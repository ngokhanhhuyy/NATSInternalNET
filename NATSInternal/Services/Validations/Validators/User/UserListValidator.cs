namespace NATSInternal.Services.Validations.Validators;

public class UserListValidator : Validator<UserListRequestDto>
{
    public UserListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotNull()
            .Must(IsEnumElementName<UserListRequestDto.FieldToBeOrdered>)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.OrderByField)));
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(Rules.PageMinimumValue)
            .WithName(dto => DisplayNames.Get(nameof(dto.Page)));
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(Rules.ResultsPerPageMinimumValue)
            .LessThanOrEqualTo(Rules.ResultsPerPageMaximumValue)
            .WithName(dto => DisplayNames.Get(nameof(dto.ResultsPerPage)));
    }

    public static class Rules
    {
        public const int PageMinimumValue = 1;
        public const int ResultsPerPageMinimumValue = 5;
        public const int ResultsPerPageMaximumValue = 50;
    }
}
