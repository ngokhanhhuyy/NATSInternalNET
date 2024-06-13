namespace NATSInternal.Services.Dtos;

public class UserAsCurrentUserResponseDto {
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public RoleBasicResponseDto Role { get; set; }
}