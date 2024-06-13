namespace NATSInternal.Services.Dtos;

public class UserUserInformationResponseDto
{
    public DateOnly? JoiningDate { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public string Note { get; set; }
    public RoleDetailResponseDto Role { get; set; }
}
