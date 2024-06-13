namespace NATSInternal.Services.Dtos;

public class UserAvatarUpsertRequestDto : IRequestDto<UserAvatarUpsertRequestDto>
{
    public bool HasChanged { get; set; }
    public byte[] Content { get; init; }

    public UserAvatarUpsertRequestDto TransformValues()
    {
        return this;
    }
}