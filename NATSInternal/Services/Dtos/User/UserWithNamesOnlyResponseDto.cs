namespace NATSInternal.Services.Dtos;

public class UserWithNamesOnlyResponseDto {
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
}