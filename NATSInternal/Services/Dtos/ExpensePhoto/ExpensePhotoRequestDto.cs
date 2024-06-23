namespace NATSInternal.Services.Dtos;

public class ExpensePhotoRequestDto : IRequestDto<ExpensePhotoRequestDto>
{
    public int? Id { get; set; }
    public byte[] File { get; set; }
    public bool HasBeenChanged { get; set; }
    
    public ExpensePhotoRequestDto TransformValues()
    {
        return this;
    }
}