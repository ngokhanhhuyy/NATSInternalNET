namespace NATSInternal.Services.Dtos;

public class OrderAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanCreatePayment { get; set; }
}