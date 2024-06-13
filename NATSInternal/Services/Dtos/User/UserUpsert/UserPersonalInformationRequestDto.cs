namespace NATSInternal.Services.Dtos;

public class UserPersonalInformationRequestDto
    : IRequestDto<UserPersonalInformationRequestDto>
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public byte[] AvatarFile { get; set; }
    public bool AvatarChanged { get; set; }

    public UserPersonalInformationRequestDto TransformValues()
    {
        FirstName = FirstName?.ToNullIfEmpty();
        MiddleName = MiddleName?.ToNullIfEmpty();
        LastName = LastName?.ToNullIfEmpty();
        PhoneNumber = PhoneNumber?.ToNullIfEmpty();
        Email = Email?.ToNullIfEmpty();
        return this;
    }
}
