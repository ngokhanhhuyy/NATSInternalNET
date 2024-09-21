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

    // Authorization for debt incurrences.
    DebtIncurrenceListAuthorizationResponseDto GetDebtIncurrenceListAuthorization();
    DebtIncurrenceAuthorizationResponseDto GetDebtIncurrenceAuthorization(DebtIncurrence debt);

    // Authorization for debt payments.
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
    bool CanSetSupplyPaidDateTime();
    bool CanAccessSupplyUpdateHistories();

    // Permissions to interact with expenses.
    bool CanCreateExpense();
    bool CanEditExpense(Expense expense);
    bool CanDeleteExpense(Expense expense);
    bool CanSetExpensePaidDateTime();
    bool CanAccessExpenseUpdateHistories();

    // Permissions to interact with orders.
    bool CanCreateOrder();
    bool CanEditOrder(Order order);
    bool CanDeleteOrder(Order order);
    bool CanSetOrderPaidDateTime();
    bool CanAccessOrderUpdateHistories();

    // Permissions to interact with treatments.
    bool CanCreateTreatment();
    bool CanEditTreatment(Treatment treatment);
    bool CanDeleteTreatment();
    bool CanSetTreatmentPaidDateTime();
    bool CanAccessTreatmentUpdateHistories();

    // Permisisons to interact with debt incurrences.
    bool CanCreateDebtIncurrence();
    bool CanEditDebtIncurrence(DebtIncurrence debt);
    bool CanDeleteDebtIncurrence();
    bool CanSetDebtIncurredDateTime();
    bool CanAccessDebtIncurrenceUpdateHistories();

    // Permissions to interact with debt payments.
    bool CanCreateDebtPayment();
    bool CanEditDebtPayment(DebtPayment debtPayment);
    bool CanDeleteDebtPayment();
    bool CanSetDebtPaymentPaidDateTime();
    bool CanAccessDebtPaymentUpdateHistories();

    // Permissions to interact with consultant.
    bool CanCreateConsultant();
    bool CanEditConsultant(Consultant consultant);
    bool CanDeleteConsultant();
    bool CanSetConsultantPaidDateTime();
    bool CanAccessConsultantUpdateHistories();
}
