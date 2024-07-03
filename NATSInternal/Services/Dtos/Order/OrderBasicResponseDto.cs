namespace NATSInternal.Services.Dtos;

public class OrderBasicResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public long Amount { get; set; }
    public bool IsClosed { get; set; }  
    public CustomerBasicResponseDto Customer { get; set; }
}