namespace NATSInternal.Services.Dtos;

public class UserPersonalInformationResponseDto
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

    public UserPersonalInformationResponseDto(User user)
    {
        FirstName = user.FirstName;
        MiddleName = user.MiddleName;
        LastName = user.LastName;
        FullName = user.FullName;
        Gender = user.Gender;
        Birthday = user.Birthday;
        PhoneNumber = user.PhoneNumber;
        Email = user.Email;
        AvatarUrl = user.AvatarUrl;
    }
}
