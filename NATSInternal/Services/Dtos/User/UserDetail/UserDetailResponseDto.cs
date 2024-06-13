namespace NATSInternal.Services.Dtos;

public class UserDetailResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public UserPersonalInformationResponseDto PersonalInformation { get; set; }
    public UserUserInformationResponseDto UserInformation { get; set; }
    public UserDetailAuthorizationResponseDto Authorization { get; set; }
}