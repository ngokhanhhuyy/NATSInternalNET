namespace NATSInternal.Services.Dtos;

public class ConsultantBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public ConsultantAuthorizationResponseDto Authorization { get; set; }
}