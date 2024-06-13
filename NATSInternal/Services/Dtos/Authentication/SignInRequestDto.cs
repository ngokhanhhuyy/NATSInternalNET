namespace NATSInternal.Services.Dtos;

public class SignInRequestDto : IRequestDto<SignInRequestDto> {
    public string UserName { get; init; }
    public string Password { get; init; }
    
    public SignInRequestDto TransformValues() {
        return this;
    }
}