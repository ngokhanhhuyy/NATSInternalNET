namespace NATSInternal.Services.Validations.Validators;

public class OrderUpsertValidator : Validator<OrderUpsertRequestDto>
{
    public OrderUpsertValidator()
    {
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.Items)
            .NotEmpty()
            .WithName(DisplayNames.OrderItem);
        RuleForEach(dto => dto.Items).SetValidator(new OrderItemValidator());
        RuleSet("Create", () =>
        {
            RuleFor(dto => dto.OrderedDateTime)
                .GreaterThanOrEqualTo(MinimumOrderedDateTime)
                .When(dto => dto.OrderedDateTime.HasValue)
                .WithName(DisplayNames.OrderedDateTime);
        });
    }

    private DateTime MinimumOrderedDateTime
    {
        get
        {
            DateTime minDate = DateTime.UtcNow.ToApplicationTime().AddMonths(-1);
            return new DateTime(minDate.Year, minDate.Month, 1, 0, 0, 0);
        }
    }
}
