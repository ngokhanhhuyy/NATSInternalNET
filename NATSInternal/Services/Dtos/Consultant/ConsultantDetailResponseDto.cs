namespace NATSInternal.Services.Dtos;

public class ConsultantDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public ConsultantAuthorizationResponseDto Authorization { get; set; }

    public ConsultantDetailResponseDto(
            Consultant consultant,
            ConsultantAuthorizationResponseDto authorization)
    {
        Id = consultant.Id;
        Amount = consultant.Amount;
        Note = consultant.Note;
        PaidDateTime = consultant.PaidDateTime;
        CreatedDateTime = consultant.CreatedDateTime;
        LastUpdatedDateTime = consultant.LastUpdatedDateTime;
        IsClosed = consultant.IsClosed;
        Customer = new CustomerBasicResponseDto(consultant.Customer);
        User = new UserBasicResponseDto(consultant.CreatedUser);
        Authorization = authorization;
    }
}