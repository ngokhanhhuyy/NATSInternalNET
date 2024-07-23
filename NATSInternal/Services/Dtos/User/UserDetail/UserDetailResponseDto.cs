namespace NATSInternal.Services.Dtos;

public class UserDetailResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public UserPersonalInformationResponseDto PersonalInformation { get; set; }
    public UserUserInformationResponseDto UserInformation { get; set; }
    public UserDetailAuthorizationResponseDto Authorization { get; set; }
    
    public UserDetailResponseDto(User user)
    {
        MapFromEntity(user);
    }

    public UserDetailResponseDto(
            User user,
            UserDetailAuthorizationResponseDto authorization)
    {
        MapFromEntity(user);
        Authorization = authorization;
    }
    
    private void MapFromEntity(User user)
    {
        Id = user.Id;
        UserName = user.UserName;
        PersonalInformation = new UserPersonalInformationResponseDto(user);
        UserInformation = new UserUserInformationResponseDto(user);
    }
}