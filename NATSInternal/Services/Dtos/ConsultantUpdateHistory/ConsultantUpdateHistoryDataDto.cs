namespace NATSInternal.Services.Dtos;

public class ConsultantUpdateHistoryDataDto
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }

    public ConsultantUpdateHistoryDataDto() { }
    
    public ConsultantUpdateHistoryDataDto(Consultant consultant)
    {
        Amount = consultant.Amount;
        Note = consultant.Note;
        PaidDateTime = consultant.PaidDateTime;
    }
}