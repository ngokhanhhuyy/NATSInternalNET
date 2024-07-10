namespace NATSInternal.Services.Validations.Validators;

public class OrderUpsertValidator : Validator<OrderUpsertRequestDto>
{
    public OrderUpsertValidator()
    {
        RuleFor(dto => dto.Note)
            .MaximumLength(255)
            .WithName(DisplayNames.Note);
        RuleFor(dto => dto.CustomerId)
            .NotEmpty()
            .WithName(DisplayNames.Customer);
        RuleFor(dto => dto.Items)
            .NotEmpty()
            .WithName(DisplayNames.Product);
        RuleForEach(dto => dto.Items)
            .SetValidator(new OrderItemValidator());
        RuleSet("Create", () =>
        {
            RuleFor(dto => dto.OrderedDateTime)
                .GreaterThanOrEqualTo(MinimumOrderedDateTime)
                .LessThanOrEqualTo(MaximumOrderedDateTime)
                .When(dto => dto.OrderedDateTime.HasValue)
                .WithName(DisplayNames.OrderedDateTime);
            RuleFor(dto => dto.Payment)
                .NotNull()
                .WithName(DisplayNames.Payment);
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

    private DateTime MaximumOrderedDateTime => DateTime.UtcNow.ToApplicationTime();
}
