namespace NATSInternal.Services.Dtos;

public class RoleRequestDto : IRequestDto<RoleRequestDto>
{
    public string Name { get; set; }

    public RoleRequestDto TransformValues()
    {
        Name = Name?.ToNullIfEmpty();
        return this;
    }
}