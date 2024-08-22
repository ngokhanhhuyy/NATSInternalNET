namespace NATSInternal.Services.Validations.Validators;

public class NotificationListValidator : Validator<NotificationListRequestDto>
{
    public NotificationListValidator()
    {
        RuleFor(dto => dto.Page)
            .GreaterThan(0)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}