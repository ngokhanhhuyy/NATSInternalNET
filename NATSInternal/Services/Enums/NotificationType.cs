﻿namespace NATSInternal.Services.Enums;

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

    // Enum values for product interaction.
    ProductCreation,
    ProductModification,
    ProductDeletion,

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

    // Enum values for debt interaction
    DebtCreation,
    DebtModification,
    DebtDeletion,

    // Enum values for debt payment interaction.
    DebtPaymentCreation,
    DebtPaymentModification,
    DebtPaymentDeletion
}