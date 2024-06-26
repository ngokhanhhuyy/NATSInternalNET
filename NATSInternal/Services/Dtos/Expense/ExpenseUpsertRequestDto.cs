namespace NATSInternal.Services.Dtos;

public class ExpenseUpsertRequestDto : IRequestDto<ExpenseUpsertRequestDto>
{
    public long Amount { get; set; }
    public DateTime? PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Note { get; set; }
    public string PayeeName { get; set; }
    public List<ExpensePhotoRequestDto> Photos { get; set; }
    
    public ExpenseUpsertRequestDto TransformValues()
    {
        PayeeName = PayeeName?.ToNullIfEmpty();
        
        if (Photos != null)
        {
            foreach (ExpensePhotoRequestDto photo in Photos)
            {
                photo?.TransformValues();
            }
        }
        
        return this;
    }
}