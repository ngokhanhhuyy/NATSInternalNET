namespace NATSInternal.Services.Validations.Validators;

public class CustomerListValidator : Validator<CustomerListRequestDto>
{
    public CustomerListValidator()
    {
        RuleFor(dto => dto.OrderByField)
            .NotEmpty()
            .Must(IsEnumElementName<CustomerListRequestDto.FieldToBeOrdered>)
            .WithMessage(ErrorMessages.Invalid)
            .WithName(dto => DisplayNames.Get(nameof(dto.OrderByField)));
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(dto => DisplayNames.Get(nameof(dto.Page)));
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(50)
            .WithName(dto => DisplayNames.Get(nameof(dto.ResultsPerPage)));
    }
}
