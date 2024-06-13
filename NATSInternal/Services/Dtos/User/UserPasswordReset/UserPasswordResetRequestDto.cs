namespace NATSInternal.Services.Dtos;

public class UserPasswordResetRequestDto : IRequestDto<UserPasswordResetRequestDto>
{
    public string NewPassword { get; set; }
    public string ConfirmationPassword { get; set; }

    public UserPasswordResetRequestDto TransformValues()
    {
        NewPassword = NewPassword?.ToNullIfEmpty();
        ConfirmationPassword = ConfirmationPassword?.ToNullIfEmpty();
        return this;
    }
}
