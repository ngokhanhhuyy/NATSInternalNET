namespace NATSInternal.Services.Dtos;

public interface IRequestDto<out TRequestDto>
{
    TRequestDto TransformValues();
}