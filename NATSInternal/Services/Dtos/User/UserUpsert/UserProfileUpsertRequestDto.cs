namespace NATSInternal.Services.Dtos;

public class UserProfileUpsertRequestDto : IRequestDto<UserProfileUpsertRequestDto>
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string IdCardNumber { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string Note { get; set; }
    public RoleRequestDto Role { get; set; }

    public UserProfileUpsertRequestDto TransformValues()
    {
        MiddleName = MiddleName?.ToNullIfEmpty();
        PhoneNumber = PhoneNumber?.ToNullIfEmpty();
        Email = Email?.ToNullIfEmpty();
        IdCardNumber = IdCardNumber?.ToNullIfEmpty();
        Note = Note?.ToNullIfEmpty();
        return this;
    }
}