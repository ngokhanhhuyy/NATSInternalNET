namespace NATSInternal.Services.Validations;

public class ValidatorLanguageManager : FluentValidation.Resources.LanguageManager {
    public ValidatorLanguageManager() {
        AddTranslation("vi", "NotNullValidator", ErrorMessages.NotNull);
        AddTranslation("vi", "NotEmptyValidator", ErrorMessages.NotEmpty);
        AddTranslation("vi", "NotEqualValidator", ErrorMessages.NotEqual);
        AddTranslation("vi", "MinLengthValidator", ErrorMessages.MinimumLength);
        AddTranslation("vi", "MaxLengthValidator", ErrorMessages.MaximumLength);
        AddTranslation("vi", "LengthValidator", ErrorMessages.StringLength);
        AddTranslation("vi", "LessThanValidator", ErrorMessages.LessThan);
        AddTranslation("vi", "LessThanOrEqualValidator", ErrorMessages.LessThanOrEqual);
        AddTranslation("vi", "GreaterThanValidator", ErrorMessages.GreaterThan);
        AddTranslation("vi", "GreaterThanOrEqualValidator", ErrorMessages.GreaterThanOrEqual);
        AddTranslation("vi", "InvalidValidator", ErrorMessages.Invalid);
        AddTranslation("vi", "NullValidator", ErrorMessages.Null);
        AddTranslation("vi", "EmailValidator", ErrorMessages.Invalid);
        AddTranslation("vi", "CreditCardValidator", ErrorMessages.Invalid);
        AddTranslation("vi", "EnumValidator", ErrorMessages.Invalid);
    }
}