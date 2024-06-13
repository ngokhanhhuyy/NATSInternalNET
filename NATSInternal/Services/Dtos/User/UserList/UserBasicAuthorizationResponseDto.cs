namespace NATSInternal.Services.Dtos;

public class UserBasicAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanChangePassword { get; set; }
    public bool CanResetPassword { get; set; }
    public bool CanDelete { get; set; }
}
