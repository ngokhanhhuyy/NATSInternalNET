using NATSInternal.Services.Dtos;

namespace NATSInternal.Services.Interfaces;

public interface IAuthorizationService
{
    Task SetUserId(int id);

    int GetUserId();

    // Authorization for users.
    UserDetailResponseDto GetUserDetail();
    UserListAuthorizationResponseDto GetUserListAuthorization();
    UserBasicAuthorizationResponseDto GetUserBasicAuthorization(User targetUser);
    UserDetailAuthorizationResponseDto GetUserDetailAuthorization(User targetUser);

    // Authorization for customers.
    CustomerListAuthorizationResponseDto GetCustomerListAuthorization();
    CustomerAuthorizationResponseDto GetCustomerAuthorization(Customer customer);

    // Authorization for brands.
    BrandListAuthorizationResponseDto GetBrandListAuthorization();
    BrandAuthorizationResponseDto GetBrandAuthorization();

    // Authorization for products.
    ProductListAuthorizationResponseDto GetProductListAuthorization();
    ProductAuthorizationResponseDto GetProductAuthorization(Product product);

    // Authorization for product categories.
    ProductCategoryAuthorizationResponseDto GetProductCategoryAuthorization();

    // Authorization for supplies.
    SupplyListAuthorizationResponseDto GetSupplyListAuthorization();
    SupplyAuthorizationResponseDto GetSupplyAuthorization(Supply supply);

    // Authorization for expenses.
    ExpenseListAuthorizationResponseDto GetExpenseListAuthorization();
    ExpenseAuthorizationResponseDto GetExpenseAuthorization(Expense expense);

    // Authorization for orders.
    OrderListAuthorizationResponseDto GetOrderListAuthorization();
    OrderAuthorizationResponseDto GetOrderAuthorization(Order order);

    // Authorization for treatments.
    TreatmentListAuthorizationResponseDto GetTreatmentListAuthorization();
    TreatmentAuthorizationResponseDto GetTreatmentAuthorization(Treatment treatment);

    // Authorization for debts.
    DebtListAuthorizationResponseDto GetDebtListAuthorization();
    DebtAuthorizationResponseDto GetDebtAuthorization(Debt debt);

    // Authorization for debt payments.
    DebtPaymentListAuthorizationResponseDto GetDebtPaymentListAuthorization();
    DebtPaymentAuthorizationResponseDto GetDebtPaymentAuthorization(DebtPayment debtPayment);

    // Authorization for consultants.
    ConsultantListAuthorizationResponseDto GetConsultantListAuthorization();
    ConsultantAuthorizationResponseDto GetConsultantAuthorization(Consultant consultant);

    // Permissions to interact with users.
    bool CanEditUserPersonalInformation(User targetUser);
    bool CanEditUserUserInformation(User targetUser);
    bool CanChangeUserPassword(User targetUser);
    bool CanResetUserPassword(User targetUser);
    bool CanDeleteUser(User targetUser);
    bool CanRestoreUser(User targetUser);
    bool CanAssignToRole(Role role);

    // Permissions to interact with supplies.
    bool CanCreateSupply();
    bool CanEditSupply(Supply supply);
    bool CanDeleteSupply(Supply supply);
    bool CanSetSupplySuppliedDateTime();
    bool CanEditSupplyItems();
    bool CanEditSupplyPhotos();

    // Permissions to interact with expenses.
    bool CanEditExpense(Expense expense);
    bool CanDeleteExpense(Expense expense);
    bool CanSetExpensePaidDateTime();

    // Permissions to interact with orders.
    bool CanEditOrder(Order order);
    bool CanDeleteOrder(Order order);
    bool CanSetOrderOrderedDateTime();

    // Permissions to interact with treatments.
    bool CanCreateTreatment();
    bool CanEditTreatment(Treatment treatment);
    bool CanDeleteTreatment();
    bool CanSetTreatmentOrderedDateTime();

    // Permisisons to interact with debts.
    bool CanCreateDebt();
    bool CanEditDebt(Debt debt);
    bool CanDeleteDebt();
    bool CanSetDebtCreatedDateTime();

    // Permissions to interact with debt payments.
    bool CanCreateDebtPayment();
    bool CanEditDebtPayment(DebtPayment debtPayment);
    bool CanDeleteDebtPayment();
    bool CanSetDebtPaymentPaidDateTime();

    // Permissions to interact with consultant.
    bool CanCreateConsultant();
    bool CanEditConsultant(Consultant consultant);
    bool CanDeleteConsultant();
    bool CanSetConsultantPaidDateTime();
}
