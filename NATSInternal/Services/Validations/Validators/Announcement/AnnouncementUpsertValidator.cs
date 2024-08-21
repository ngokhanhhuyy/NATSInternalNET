namespace NATSInternal.Services.Validations.Validators;

public class AnnouncementUpsertValidator : Validator<AnnouncementUpsertRequestDto>
{
    public AnnouncementUpsertValidator()
    {
        RuleFor(dto => dto.Category)
            .IsInEnum()
            .WithName(DisplayNames.Category);
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MaximumLength(80)
            .WithName(DisplayNames.Title);
        RuleFor(dto => dto.Content)
            .NotEmpty()
            .MaximumLength(5000)
            .WithName(DisplayNames.Content);
        RuleSet("Create", () =>
        {
            RuleFor(dto => dto.IntervalInMinutes)
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(30 * 24 * 60)
                .WithName(DisplayNames.IntervalInMinutes);
        });
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.IntervalInMinutes)
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(30 * 24 * 60)
                .When(dto => dto.StartingDateTime.HasValue)
                .WithName(DisplayNames.IntervalInMinutes);
        });
    }
}