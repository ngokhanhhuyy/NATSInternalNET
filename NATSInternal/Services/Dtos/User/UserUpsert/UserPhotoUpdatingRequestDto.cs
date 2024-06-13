namespace NATSInternal.Services.Dtos;

public class UserPhotoUpdatingRequestDto : IRequestDto<UserPhotoUpdatingRequestDto>
{
    public string Operation { get; set; }
    public int? Id { get; set; }
    public byte[] Content { get; set; }

    public UserPhotoUpdatingRequestDto TransformValues()
    {
        return this;
    }

    public enum OperationName
    {
        Create,
        Replace,
        Delete
    }

}
