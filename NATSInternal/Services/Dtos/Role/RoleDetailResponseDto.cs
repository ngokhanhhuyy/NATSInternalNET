namespace NATSInternal.Services.Dtos;

public class RoleDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int PowerLevel { get; set; }
    public List<string> Permissions { get; set; }
}
