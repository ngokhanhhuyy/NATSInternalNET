namespace NATSInternal.Services.Validations.Validators;

public class AnnouncementListValidator : Validator<AnnouncementListRequestDto>
{
    public AnnouncementListValidator()
    {
        RuleFor(dto => dto.Page)
            .GreaterThanOrEqualTo(1)
            .WithName(DisplayNames.Page);
        RuleFor(dto => dto.ResultsPerPage)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50)
            .WithName(DisplayNames.ResultsPerPage);
    }
}