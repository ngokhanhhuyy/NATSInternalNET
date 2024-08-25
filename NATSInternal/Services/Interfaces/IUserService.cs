namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service class to handle users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get a list of users with pagination, filtering and sorting options.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing all the options for the list results.
    /// </param>
    /// <returns>
    /// An object containing the results and page count (for pagination
    /// calculation).
    /// </returns>
    Task<UserListResponseDto> GetListAsync(UserListRequestDto requestDto);
    
    /// <summary>
    /// Get a list of users who have just joined (within 1 month from
    /// joining date).
    /// </summary>
    /// <returns>
    /// An object containing the users who have just joined.
    /// </returns>
    Task<UserListResponseDto> GetJoinedRecentlyListAsync();

    /// <summary>
    /// Get a list of users who have upcoming birthday (within
    /// 1 month from now).
    /// </summary>
    /// <returns>
    /// An object containing the users who have upcoming birthday.
    /// </returns>
    Task<UserListResponseDto> GetUpcomingBirthdayListAsync();

    /// <summary>
    /// Get the role information which is associated to the user
    /// with given id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>The full detail of the user's role.</returns>
    Task<RoleDetailResponseDto> GetRoleAsync(int id);

    /// <summary>
    /// Get fully detailed information, including role, claims
    /// (permissions) of the user with given id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>An object containing all details of the user.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with the given id cannot be found in the database.
    /// </exception>
    Task<UserDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create user with given information.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing all the information for a new user.
    /// </param>
    /// <returns>The id of the created user.</returns>
    /// <exception cref="DuplicatedException">
    /// The username in the provided data already exists.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// The role name in the provided data doesn't exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to
    /// create a new user.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Business rules violation during assign the new user to
    /// the specified role.
    /// </exception>
    Task<int> CreateAsync(UserCreateRequestDto requestDto);

    /// <summary>
    /// Update a user with given id.
    /// </summary>
    /// <param name="id">The id of the user to be updated.</param>
    /// <param name="requestDto">
    /// An object containing new data to be updated.
    /// </param>
    /// <returns>
    /// A Task object representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have enough permission to
    /// perform the updating action.
    /// </exception>
    Task UpdateAsync(int id, UserUpdateRequestDto requestDto);

    /// <summary>
    /// Change the password of the user with given id.
    /// </summary>
    /// <param name="id">
    /// The id of the user to be changed his password.
    /// </param>
    /// <param name="requestDto">
    /// An object containing the current password, the new
    /// one and the confirmation one.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have enough permission
    /// to perform this action.
    /// </exception>
    /// <exception cref="OperationException">
    /// The current password is incorrect.
    /// </exception>
    Task ChangePasswordAsync(int id, UserPasswordChangeRequestDto requestDto);

    /// <summary>
    /// Reset the password of the user with given id (without
    /// the need of providing the current password).
    /// </summary>
    /// <param name="id">
    /// The id of the user to be reset password.
    /// </param>
    /// <param name="requestDto">
    /// An object containing new password and confirmation password.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to
    /// perform this action.
    /// </exception>
    /// <exception cref="OperationException">
    /// New password's complexity doesn't meet requirement.
    /// </exception>
    Task ResetPasswordAsync(int id, UserPasswordResetRequestDto requestDto);
    
    /// <summary>
    /// Delete the user with given id.
    /// </summary>
    /// <param name="id">The id of the user to be deleted.</param>
    /// <returns>
    /// A Task object representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to
    /// perform this action.
    /// </exception>
    Task DeleteAsync(int id);

    /// <summary>
    /// Restore the user with given id who has been marked as deleted.
    /// </summary>
    /// <param name="id">The id of the user to be restored.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user who has given id and is marked as deleted cannot
    /// be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to
    /// perform this action.
    /// </exception>
    Task RestoreAsync(int id);
}
