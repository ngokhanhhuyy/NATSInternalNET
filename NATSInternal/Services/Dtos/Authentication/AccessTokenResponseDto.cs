namespace NATSInternal.Services.Dtos;

public class AccessTokenResponseDto {
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required int UserId { get; set; }
    public required DateTime ExpiringDateTime { get; set; }
}