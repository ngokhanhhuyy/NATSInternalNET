namespace NATSInternal.Models;

public class UserUserInformationModel
{
    public DateOnly? JoiningDate { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public string Note { get; set; }
    public RoleDetailModel Role { get; set; }

    public static UserUserInformationModel FromResponseDto(UserUserInformationResponseDto responseDto)
    {
        return new UserUserInformationModel
        {
            JoiningDate = responseDto.JoiningDate,
            CreatedDateTime = responseDto.CreatedDateTime,
            UpdatedDateTime = responseDto.UpdatedDateTime,
            Note = responseDto.Note,
            Role = RoleDetailModel.FromResponseDto(responseDto.Role)
        };
    }
}