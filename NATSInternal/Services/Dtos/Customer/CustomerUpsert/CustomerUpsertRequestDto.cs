namespace NATSInternal.Services.Dtos;

public class CustomerUpsertRequestDto : IRequestDto<CustomerUpsertRequestDto> {
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string NickName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string ZaloNumber { get; set; }
    public string FacebookUrl { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Note { get; set; }
    public int? IntroducerId { get; set; }

    public CustomerUpsertRequestDto TransformValues() {
        FirstName = FirstName?.ToNullIfEmpty();
        MiddleName = MiddleName?.ToNullIfEmpty();
        LastName = LastName?.ToNullIfEmpty();
        NickName = NickName?.ToNullIfEmpty();
        PhoneNumber = PhoneNumber?.ToNullIfEmpty();
        ZaloNumber = ZaloNumber?.ToNullIfEmpty();
        FacebookUrl = FacebookUrl?.ToNullIfEmpty();
        Email = Email?.ToNullIfEmpty();
        Address = Address?.ToNullIfEmpty();
        Note = Note?.ToNullIfEmpty();
        IntroducerId = !IntroducerId.HasValue || IntroducerId.Value == 0
            ? null : IntroducerId.Value;
        return this;
    }
}