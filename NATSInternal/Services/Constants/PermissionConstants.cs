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
    public const string EditClosedSupply = "EditClosedSupply";
    public const string DeleteSupply = "DeleteSupply";

    // Permissions to interact with supply items.
    public const string EditSupplyItem = "EditSupplyItem";
    public const string DeleteSupplyItem = "DeleteSupplyItem";

    // Permissions to interact with supply photos.
    public const string EditSupplyPhoto = "EditSupplyPhoto";
    public const string DeleteSupplyPhoto = "DeleteSupplyPhoto";
    
    // Permissions to interact with expense.
    public const string CreateExpense = "CreateExpense";
    public const string EditExpense = "EditExpense";
    public const string EditClosedExpense = "EditClosedExpense";
    public const string DeleteExpense = "DeleteExpense";
    

}
