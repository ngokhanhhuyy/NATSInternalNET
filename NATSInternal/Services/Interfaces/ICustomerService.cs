namespace NATSInternal.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerListResponseDto> GetListAsync(CustomerListRequestDto requestDto);

    Task<CustomerBasicResponseDto> GetBasicAsync(int id);

    Task<CustomerDetailResponseDto> GetDetailAsync(int id);

    Task<CustomerCreateResponseDto> CreateAsync(CustomerUpsertRequestDto requestDto);

    Task UpdateAsync(int id, CustomerUpsertRequestDto requestDto);

    Task DeleteAsync(int id);
}