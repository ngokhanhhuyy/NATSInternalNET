namespace NATSInternal.Components;

public class CurrentUserComponent : ViewComponent
{
    private IAuthorizationService _authorizationService;

    public CurrentUserComponent(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public IViewComponentResult Invoke()
    {
        UserDetailResponseDto responseDto = _authorizationService.GetUserDetail();
        UserDetailModel model = UserDetailModel.FromResponseDto(responseDto);
        return View("/Views/Shared/_CurrentUser.cshtml", model);
    }
}