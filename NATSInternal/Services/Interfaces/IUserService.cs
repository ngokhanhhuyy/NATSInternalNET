namespace NATSInternal.Services.Interfaces;

public interface IUserService
{
    Task<UserListResponseDto> GetListAsync(UserListRequestDto requestDto);

    Task<UserListResponseDto> GetJoinedRecentlyListAsync();

    Task<UserListResponseDto> GetUpcomingBirthdayListAsync();

    Task<RoleDetailResponseDto> GetRoleAsync(int id);

    Task<UserDetailResponseDto> GetDetailAsync(int id);

    Task<UserCreateResponseDto> CreateAsync(UserCreateRequestDto requestDto);

    Task UpdateAsync(int id, UserUpdateRequestDto requestDto);

    Task ChangePasswordAsync(int id, UserPasswordChangeRequestDto requestDto);

    Task ResetPasswordAsync(int id, UserPasswordResetRequestDto requestDto);

    Task DeleteAsync(int id);

    Task RestoreAsync(int id);
}
