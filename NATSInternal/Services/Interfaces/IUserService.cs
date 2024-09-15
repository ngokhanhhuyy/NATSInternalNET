namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service class to handle user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrives a list of user with basic information, based on the specified filtering,
    /// sorting and paginating conditions.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="UserListRequestDto"/> class, containing the conditions
    /// for the results.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="UserListResponseDto"/> class, containing the results and
    /// the additional information for pagination.
    /// </returns>
    Task<UserListResponseDto> GetListAsync(UserListRequestDto requestDto);

    /// <summary>
    /// Retrieves a list of user with basic information, based on the specified ids.
    /// </summary>
    /// <param name="ids">
    /// An instance of the <see cref="IEnumerable{T}"/> implementation where <c>T</c> is
    /// <see cref="int"/>, representing the ids of the customers to retrieve.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="UserListResponseDto"/> class, containing the results.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when there is any user with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <remarks>
    /// This method's results don't contain neither the information for pagination nor the
    /// authorization information. It only returns the results containing the users with the
    /// exactly ids specified in the parameter if all of the users are found. At that time,
    /// the value of the property <c>PageCount</c> in the <see cref="UserListResponseDto"/>
    /// instance will always be 1 and the value of the property <c>Authorization</c> will
    /// always be null.
    /// </remarks>
    Task<UserListResponseDto> GetListAsync(IEnumerable<int> ids);

    /// <summary>
    /// Retrieves a list of users who have just joined (within 1 month from joining date) with
    /// the basic information.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="UserListResponseDto"/>, containing the results.
    /// </returns>
    Task<UserListResponseDto> GetJoinedRecentlyListAsync();

    /// <summary>
    /// Retrieves a list of users who have incoming birthdays (within 1 month from now) with
    /// the basic information.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="UserListResponseDto"/> class, containing the results.
    /// </returns>
    Task<UserListResponseDto> GetUpcomingBirthdayListAsync();

    /// <summary>
    /// Retrieves a specific user's role details, specified by the user's id.
    /// </summary>
    /// <param name="id">An <see cref="int"/> representing the id of the user.</param>
    /// <returns>
    /// An instance of the <see cref="RoleDetailResponseDto"/> class, containing the details
    /// of the role to retrieve.
    /// </returns>
    Task<RoleDetailResponseDto> GetRoleAsync(int id);

    /// <summary>
    /// Retrieves the basic information of a specific user, specified by the user's id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the user to retrieve.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="UserBasicResponseDto"/> class, containing the
    /// basic information of the retrieving user.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified <c>id</c> doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task<UserBasicResponseDto> GetBasicAsync(int id);

    /// <summary>
    /// Get the details of a specific user, specified by the id of the user.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the user to retrieve.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="UserDetailResponseDto"/> class, containing the details
    /// of the user.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id doesn't exist or has already been deleted.
    /// </exception>
    Task<UserDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new user, based on the specified data.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="UserCreateRequestDto"/> class, containing the data for
    /// the new user.
    /// </param>
    /// <returns>An <see cref="int"/> representing the id of the new user.</returns>
    /// <exception cref="DuplicatedException">
    /// Throws when the username specified in the argument for the <c>requestDto</c> parameter
    /// already exists.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the name of the role, specified by the value of the property
    /// <c>UserInformation.Role.Name</c> in the argument for the <c>requestDto</c> parameter
    /// doesn't exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to create a new user.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Throws when a business rules violation occurs during the assignment of the new user to
    /// the specified role.
    /// </exception>
    Task<int> CreateAsync(UserCreateRequestDto requestDto);

    /// <summary>
    /// Updates an existing user, based on the specified id and data.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the user to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="UserUpdateRequestDto"/> class, containg the data for the
    /// updating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to update the target
    /// user.
    /// </exception>
    Task UpdateAsync(int id, UserUpdateRequestDto requestDto);

    /// <summary>
    /// Changes the password of the user with the specified id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the target user.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="UserPasswordChangeRequestDto"/> class, contaning the
    /// current password, the new password and the confirmation password for the operation.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// The requesting user and the user whose id is specified by the value of the `id`
    /// argument must be the same one.
    /// </remarks>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id doens't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user isn't the target user.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the current password, provided in the <c>requestDto</c> is incorrect.
    /// </exception>
    Task ChangePasswordAsync(int id, UserPasswordChangeRequestDto requestDto);

    /// <summary>
    /// Resets the password of the user, specified by the id, without the need of providing
    /// the current password.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the target user.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="UserPasswordResetRequestDto"/> class, contanining the
    /// new password and the confirmation password for the operation.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user is actually the target user, or doesn't have enough
    /// permissions to reset the target user's password.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the specified new password's complexity doesn't meet the requirement.
    /// </exception>
    Task ResetPasswordAsync(int id, UserPasswordResetRequestDto requestDto);

    /// <summary>
    /// Deletes the user with the specified id.
    /// </summary>
    /// <param name="id">An <see cref="int"/> representing the id of the target user.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to delete the target
    /// user.
    /// </exception>
    Task DeleteAsync(int id);

    /// <summary>
    /// Restores the user with the specified id who has been soft-deleted.
    /// </summary>
    /// <param name="id">An <see cref="int"/> representing the id of the target user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the user with the specified id hasn't been soft-deleted or has already
    /// been deleted entirely from the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to perform the
    /// operation.
    /// </exception>
    Task RestoreAsync(int id);
}
