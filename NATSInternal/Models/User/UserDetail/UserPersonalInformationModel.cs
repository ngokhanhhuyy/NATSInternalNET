namespace NATSInternal.Models;

public class UserPersonalInformationModel
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }

    public static UserPersonalInformationModel FromResponseDto(
            UserPersonalInformationResponseDto responseDto)
    {
        return new UserPersonalInformationModel
        {
            FirstName = responseDto.FirstName,
            MiddleName = responseDto.MiddleName,
            LastName = responseDto.LastName,
            FullName = responseDto.FullName,
            Gender = responseDto.Gender,
            Birthday = responseDto.Birthday,
            PhoneNumber = responseDto.PhoneNumber,
            Email = responseDto.Email,
            AvatarUrl = responseDto.AvatarUrl
        };
    }
}