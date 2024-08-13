namespace NATSInternal.Services.Dtos;

public record CustomerDetailResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string ZaloNumber { get; set; }
    public string FacebookUrl { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public CustomerBasicResponseDto Introducer { get; set; }
    public long DebtRemainingAmount { get; set; }

    public CustomerDetailResponseDto(
            Customer customer,
            CustomerAuthorizationResponseDto authorization)
    {
        Id = customer.Id;
        FirstName = customer.FirstName;
        MiddleName = customer.MiddleName;
        LastName = customer.LastName;
        FullName = customer.FullName;
        NickName = customer.NickName;
        Gender = customer.Gender;
        Birthday = customer.Birthday;
        PhoneNumber = customer.PhoneNumber;
        ZaloNumber = customer.ZaloNumber;
        FacebookUrl = customer.FacebookUrl;
        Email = customer.Email;
        Address = customer.Address;
        Note = customer.Note;
        CreatedDateTime = customer.CreatedDateTime;
        UpdatedDateTime = customer.UpdatedDateTime;
        DebtRemainingAmount = customer.DebtRemainingAmount;

        if (customer.Introducer != null)
        {
            Introducer = new CustomerBasicResponseDto(customer.Introducer);
        }
    }
}