namespace NATSInternal.Services.Dtos;

public class UserAccountUpsertRequestDto : IRequestDto<UserAccountUpsertRequestDto>
{
    public string UserName { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmationPassword { get; set; }

    public UserAccountUpsertRequestDto TransformValues()
    {
        UserName = UserName?.ToNullIfEmpty();
        CurrentPassword = UserName?.ToNullIfEmpty();
        NewPassword = UserName?.ToNullIfEmpty();
        ConfirmationPassword = UserName?.ToNullIfEmpty();
        return this;
    }
}