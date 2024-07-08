namespace NATSInternal.Services.Interfaces;

public interface IAuthorizationService
{
    Task SetUserId(int id);

    int GetUserId();

    UserDetailResponseDto GetUserDetail();

    UserAuthorizationResponseDto GetUserAuthorization();

    UserBasicAuthorizationResponseDto GetUserBasicAuthorization(User targetUser);

    UserDetailAuthorizationResponseDto GetUserDetailAuthorization(User targetUser);

    BrandAuthorizationResponseDto GetBrandAuthorization();

    ProductCategoryAuthorizationResponseDto GetProductCategoryAuthorization();

    SupplyDetailAuthorizationResponseDto GetSupplyDetailAuthorization(Supply supply);

    ExpenseAuthorizationResponseDto GetExpenseAuthorization(Expense expense);

    OrderListAuthorizationResponseDto GetOrderListAuthorization();

    OrderAuthorizationResponseDto GetOrderAuthorization(Order order);

    OrderPaymentAuthorizationResponseDto GetOrderPaymentAuthorization(OrderPayment orderPayment);

    bool CanEditUserPersonalInformation(User targetUser);

    bool CanEditUserUserInformation(User targetUser);

    bool CanChangeUserPassword(User targetUser);

    bool CanResetUserPassword(User targetUser);

    bool CanDeleteUser(User targetUser);

    bool CanRestoreUser(User targetUser);

    bool CanAssignToRole(Role role);

    bool CanEditSupply(Supply supply);

    public bool CanDeleteSupply(Supply supply);

    bool CanEditSupplyItems();

    bool CanEditSupplyPhotos();
    
    bool CanEditExpense(Expense expense);
    
    bool CanDeleteExpense(Expense expense);
    
    bool CanEditOrder(Order order);
    
    bool CanDeleteOrder(Order order);

    bool CanEditOrderPayment(OrderPayment payment);

    bool CanDeleteOrderPayment(OrderPayment payment);
}
