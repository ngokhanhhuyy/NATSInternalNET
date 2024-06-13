namespace NATSInternal.Models;

public class RoleDetailModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int PowerLevel { get; set; }
    public List<string> Permissions { get; set; }

    public static RoleDetailModel FromResponseDto(RoleDetailResponseDto responseDto)
    {
        return new RoleDetailModel
        {
            Id = responseDto.Id,
            Name = responseDto.Name,
            DisplayName = responseDto.DisplayName,
            PowerLevel = responseDto.PowerLevel,
            Permissions = responseDto.Permissions
        };
    }
}