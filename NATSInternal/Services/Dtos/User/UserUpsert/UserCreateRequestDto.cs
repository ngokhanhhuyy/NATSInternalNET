namespace NATSInternal.Services.Dtos;

public class UserCreateRequestDto : IRequestDto<UserCreateRequestDto>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConfirmationPassword { get; set; }
    public UserPersonalInformationRequestDto PersonalInformation { get; set; }
    public UserUserInformationRequestDto UserInformation { get; set; }

    public UserCreateRequestDto TransformValues()
    {
        UserName = UserName?.ToNullIfEmpty();
        Password = Password?.ToNullIfEmpty();
        ConfirmationPassword = ConfirmationPassword?.ToNullIfEmpty();
        PersonalInformation = PersonalInformation.TransformValues();
        UserInformation = UserInformation.TransformValues();
        return this;
    }
}