namespace NATSInternal.Models;

public class UserDetailModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public UserPersonalInformationModel PersonalInformation { get; set; }
    public UserUserInformationModel UserInformation { get; set; }

    public static UserDetailModel FromResponseDto(UserDetailResponseDto responseDto)
    {
        return new UserDetailModel
        {
            Id = responseDto.Id,
            UserName = responseDto.UserName,
            PersonalInformation = UserPersonalInformationModel
                .FromResponseDto(responseDto.PersonalInformation),
            UserInformation = UserUserInformationModel
                .FromResponseDto(responseDto.UserInformation)
        };
    }
}