namespace NATSInternal.Services.Enums;

public enum NotificationType
{
    // Enum values for user interactions.
    UserCreation,
    UserModification,
    UserDeletion,
    UserBirthday,
    UserJoiningDateAnniversary,

    // Enum values for customer interaction.
    CustomerCreation,
    CustomerModification,
    CustomerDeletion,
    CustomerBirthday,
    
    // Enum values for brand interaction.
    BrandCreation,
    BrandModification,
    BrandDeletion,

    // Enum values for product interaction.
    ProductCreation,
    ProductModification,
    ProductDeletion,
    
    // Enum values for product category interaction.
    ProductCategoryCreation,
    ProductCategoryModification,
    ProductCategoryDeletion,

    // Enum values for expense interaction.
    ExpenseCreation,
    ExpenseModification,
    ExpenseDeletion,

    // Enum values for supply interaction.
    SupplyCreation,
    SupplyModification,
    SupplyDeletion,

    // Enum values for consultant interaction.
    ConsultantCreation,
    ConsultantModification,
    ConsultantDeletion,

    // Enum values for order interaction.
    OrderCreation,
    OrderModification,
    OrderDeletion,

    // Enum values for treatment interaction.
    TreatmentCreation,
    TreatmentModification,
    TreatmentDeletion,

    // Enum values for debt incurrence interaction
    DebtIncurrenceCreation,
    DebtIncurrenceModification,
    DebtIncurrenceDeletion,

    // Enum values for debt payment interaction.
    DebtPaymentCreation,
    DebtPaymentModification,
    DebtPaymentDeletion,
    
    // Enum values for announcements.
    AnnouncementCreation,
    AnnouncementModification,
    AnnouncementDeletion
}
