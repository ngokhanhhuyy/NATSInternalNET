namespace NATSInternal.Services.Dtos;

public class UserDetailAuthorizationResponseDto
{
    public bool CanGetNote { get; set; }
    public bool CanEdit { get; set; }
    public bool CanEditUserPersonalInformation { get; set; }
    public bool CanEditUserUserInformation { get; set; }
    public bool CanAssignRole { get; set; }
    public bool CanChangePassword { get; set; }
    public bool CanResetPassword { get; set; }
    public bool CanDelete { get; set; }
}
