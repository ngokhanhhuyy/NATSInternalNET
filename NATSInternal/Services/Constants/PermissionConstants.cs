namespace NATSInternal.Services.Constants;

public static class PermissionConstants
{
    // Permissions to interact with users.
    public const string CreateUser = "CreateUser";
    public const string GetOtherUserPersonalInformation = "GetOtherUserPersonalInformation";
    public const string GetOtherUserUserInformation = "GetOtherUserUserInformation";
    public const string GetOtherUserNote = "GetOtherUserNote";
    public const string EditSelfPersonalInformation = "EditSelfPersonalInformation";
    public const string EditSelfUserInformation = "EditSelfUserInformation";
    public const string EditOtherUserPersonalInformation = "EditOtherUserPersonalInformation";
    public const string EditOtherUserUserInformation = "EditOtherUserUserInformation";
    public const string AssignRole = "AssignRole";
    public const string ResetOtherUserPassword = "ResetOtherUserPassword";
    public const string DeleteUser = "DeleteUser";
    public const string RestoreUser = "RestoreUser";

    // Permissions to interact with customers.
    public const string GetCustomerDetail = "GetCustomerDetail";
    public const string CreateCustomer = "CreateCustomer";
    public const string EditCustomer = "EditCustomer";
    public const string DeleteCustomer = "DeleteCustomer";

    // Permissions to interact with brands.
    public const string CreateBrand = "CreateBrand";
    public const string EditBrand = "EditBrand";
    public const string DeleteBrand = "DeleteBrand";

    // Permissions to interact with products
    public const string CreateProduct = "CreateProduct";
    public const string EditProduct = "EditProduct";
    public const string DeleteProduct = "DeleteProduct";

    // Permissions to interact with product categories
    public const string CreateProductCategory = "CreateProductCategory";
    public const string EditProductCategory = "EditProductCategory";
    public const string DeleteProductCategory = "DeleteProductCategory";

    // Permissions to interact with supplies.
    public const string CreateSupply = "CreateSupply";
    public const string EditSupply = "EditSupply";
    public const string EditLockedSupply = "EditLockedSupply";
    public const string DeleteSupply = "DeleteSupply";
    public const string SetSupplyPaidDateTime = "SetSupplyPaidDateTime";
    public const string AccessSupplyUpdateHistories = "AccessSupplyUpdateHistories";
    
    // Permissions to interact with expenses.
    public const string CreateExpense = "CreateExpense";
    public const string EditExpense = "EditExpense";
    public const string EditLockedExpense = "EditLockedExpense";
    public const string DeleteExpense = "DeleteExpense";
    public const string SetExpensePaidDateTime = "SetExpensePaidDateTime";
    public const string AccessExpenseUpdateHistories = "AccessExpenseUpdateHistories";
    
    // Permissions to interact with orders.
    public const string CreateOrder = "CreateOrder";
    public const string EditOrder = "EditOrder";
    public const string EditLockedOrder = "EditLockedOrder";
    public const string SetOrderPaidDateTime = "SetOrderPaidDateTime";
    public const string DeleteOrder = "DeleteOrder";
    public const string AccessOrderUpdateHistories = "AccessOrderUpdateHistories";
    
    // Permissions to interact with treatments.
    public const string CreateTreatment = "CreateTreatment";
    public const string EditTreatment = "EditTreatment";
    public const string EditLockedTreatment = "EditLockedTreatment";
    public const string DeleteTreatment = "DeleteTreatment";
    public const string SetTreatmentPaidDateTime = "SetTreatmentPaidDateTime";
    public const string AccessTreatmentUpdateHistories = "AccessTreatmentUpdateHistories";

    // Permissions to interact with debts.
    public const string CreateDebt = "CreateDebt";
    public const string EditDebt = "EditDebt";
    public const string EditLockedDebt = "EditLockedDebt";
    public const string SetDebtIncurredDateTime = "SetDebtIncurredDateTime";
    public const string DeleteDebt = "DeleteDebt";
    public const string AccessDebtUpdateHistories = "AccessDebtUpdateHistories";

    // Permissions to interact with debt payments.
    public const string CreateDebtPayment = "CreateDebtPayment";
    public const string EditDebtPayment = "EditDebtPayment";
    public const string EditLockedDebtPayment = "EditLockedDebtPayment";
    public const string SetDebtPaymentPaidDateTime = "SetDebtPaymentPaidDateTime";
    public const string DeleteDebtPayment = "DeleteDebtPayment";
    public const string AccessDebtPaymentUpdateHistories = "AccessDebtPaymentUpdateHistories";

    // Permissions to interact with consultants.
    public const string CreateConsultant = "CreateConsultant";
    public const string EditConsultant = "EditConsultant";
    public const string EditLockedConsultant = "EditLockedConsultant";
    public const string DeleteConsultant = "DeleteConsultant";
    public const string SetConsultantPaidDateTime = "SetConsultantPaidDateTime";
    public const string AccessConsultantUpdateHistories = "AccessConsultantUpdateHistories";
}
