namespace NATSInternal.Models;

public class RoleBasicModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int PowerLevel { get; set; }

    public RoleRequestDto ToRequestDto()
    {
        return new RoleRequestDto
        {
            Name = Name
        };
    }

    public static RoleBasicModel FromResponseDto(RoleBasicResponseDto responseDto)
    {
        return new RoleBasicModel
        {
            Id = responseDto.Id,
            Name = responseDto.Name,
            DisplayName = responseDto.DisplayName,
            PowerLevel = responseDto.PowerLevel
        };
    }
}