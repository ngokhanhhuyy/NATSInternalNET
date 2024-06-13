namespace NATSInternal.Services.Dtos;

public class CustomerBasicResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
}