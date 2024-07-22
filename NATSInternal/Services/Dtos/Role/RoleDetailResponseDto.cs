namespace NATSInternal.Services.Dtos;

public class RoleDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int PowerLevel { get; set; }
    public List<string> Permissions { get; set; }

    public RoleDetailResponseDto(Role role)
    {
        Id = role.Id;
        Name = role.Name;
        DisplayName = role.DisplayName;
        PowerLevel = role.PowerLevel;
        Permissions = role.Claims
            .Where(c => c.ClaimType == "Permission")
            .Select(c => c.ClaimValue)
            .ToList();
    }
}
