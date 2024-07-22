namespace NATSInternal.Services.Dtos;

public class ExpensePayeeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ExpensePayeeResponseDto(ExpensePayee payee)
    {
        Id = payee.Id;
        Name = payee.Name;
    }
}