namespace NATSInternal.Services.Dtos;

public class UserPasswordChangeRequestDto : IRequestDto<UserPasswordChangeRequestDto>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmationPassword { get; set; }

    public UserPasswordChangeRequestDto TransformValues()
    {
        CurrentPassword = CurrentPassword?.ToNullIfEmpty();
        NewPassword = NewPassword?.ToNullIfEmpty();
        ConfirmationPassword = ConfirmationPassword?.ToNullIfEmpty();
        return this;
    }
}
