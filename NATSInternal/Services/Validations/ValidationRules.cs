namespace NATSInternal.Services.Validations;

public static class ValidationRules {
    public static class User {
        public const int UserNameMaximumLength = 20;
        public const int PasswordMinimumLength = 8;
        public const int PasswordMaximumLength = 20;
        public const int ConfirmationPasswordMaximumLength = 20;
        public const int FirstNameMaximumLength = 15;
        public const int MiddleNameMaximumLength = 20;
        public const int LastNameMaximumLength = 15;
        public const int PhoneNumberMaximumLength = 12;
        public const int IdCardNumberMaximumLength = 12;
        public const int NoteMaximumLength = 255;
    }
}