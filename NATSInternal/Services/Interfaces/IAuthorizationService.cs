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

    bool CanEditUserPersonalInformation(User targetUser);

    bool CanEditUserUserInformation(User targetUser);

    bool CanChangeUserPassword(User targetUser);

    bool CanResetUserPassword(User targetUser);

    bool CanDeleteUser(User targetUser);

    bool CanRestoreUser(User targetUser);

    bool CanAssignToRole(Role role);

    bool CanEditSupplyItems();

    bool CanEditSupplyPhotos();
}
