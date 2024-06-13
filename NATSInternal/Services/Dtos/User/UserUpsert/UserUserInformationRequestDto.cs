namespace NATSInternal.Services.Dtos;

public class UserUserInformationRequestDto
    : IRequestDto<UserUserInformationRequestDto>
{
    public DateOnly? JoiningDate { get; set; }
    public string Note { get; set; }
    public RoleRequestDto Role { get; set; }

    public UserUserInformationRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        return this;
    }
}
