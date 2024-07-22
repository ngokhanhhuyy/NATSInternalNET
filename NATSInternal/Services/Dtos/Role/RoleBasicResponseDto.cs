namespace NATSInternal.Services.Dtos;

public class RoleBasicResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int PowerLevel { get; set; }

    public RoleBasicResponseDto(Role role)
    {
        Id = role.Id;
        Name = role.Name;
        DisplayName = role.DisplayName;
        PowerLevel = role.PowerLevel;
    }
}