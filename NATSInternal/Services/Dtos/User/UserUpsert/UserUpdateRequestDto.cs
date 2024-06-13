namespace NATSInternal.Services.Dtos;

public class UserUpdateRequestDto : IRequestDto<UserUpdateRequestDto>
{
    public UserPersonalInformationRequestDto PersonalInformation { get; set; }
    public UserUserInformationRequestDto UserInformation { get; set; }

    public UserUpdateRequestDto TransformValues()
    {
        return this;
    }
}
