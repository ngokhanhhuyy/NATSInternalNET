using Microsoft.EntityFrameworkCore.Storage;

namespace NATSInternal.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPhotoService _photoService;
    private readonly SqlExceptionHandler _exceptionHandler;

    public UserService(
            DatabaseContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IAuthorizationService authorizationService,
            IPhotoService photoService,
            SqlExceptionHandler exceptionHandler)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _authorizationService = authorizationService;
        _photoService = photoService;
        _exceptionHandler = exceptionHandler;
    }

    /// <summary>
    /// Get a list of users with pagination, filtering and sorting options.
    /// </summary>
    /// <param name="requestDto">An object containing all the options for the list results.</param>
    /// <returns>An object containing the results and page count (for pagination calculation).</returns>
    public async Task<UserListResponseDto> GetListAsync(UserListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<User> query = _context.Users
            .Include(u => u.Roles)
            .Where(u => !u.IsDeleted);

        // Determine the field and the direction to sort.
        switch (requestDto.OrderByField)
        {
            case nameof(UserListRequestDto.FieldToBeOrdered.FirstName):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.FirstName)
                    : query.OrderByDescending(u => u.FirstName);
                break;
            case nameof(UserListRequestDto.FieldToBeOrdered.UserName):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.UserName)
                    : query.OrderByDescending(u => u.UserName);
                break;
            case nameof(UserListRequestDto.FieldToBeOrdered.Birthday):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.Birthday)
                    : query.OrderByDescending(u => u.Birthday);
                break;
            case nameof(UserListRequestDto.FieldToBeOrdered.CreatedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.CreatedDateTime)
                    : query.OrderByDescending(u => u.CreatedDateTime);
                break;
            case nameof(UserListRequestDto.FieldToBeOrdered.Role):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.Roles.First().Id)
                    : query.OrderByDescending(u => u.Roles.First().Id);
                break;
            case nameof(UserListRequestDto.FieldToBeOrdered.LastName):
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(u => u.LastName)
                    : query.OrderByDescending(u => u.LastName);
                break;
        }

        // Filter by role.
        if (requestDto.RoleId.HasValue)
        {
            query = query
                .Where(u => u.Roles.First().Id == requestDto.RoleId.Value);
        }

        // Filter by search content
        if (requestDto.Content != null && requestDto.Content.Length >= 3)
        {
            query = query
                .Where(u =>
                    u.FullName.Contains(
                        requestDto.Content,
                        StringComparison.CurrentCultureIgnoreCase) ||
                    u.UserName.Contains(
                        requestDto.Content,
                        StringComparison.CurrentCultureIgnoreCase));
        }

        // Initialize response dto.
        UserListResponseDto responseDto = new UserListResponseDto
        {
            Authorization = _authorizationService.GetUserListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Results = await query
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .Select(u => new UserBasicResponseDto(
                u,
                _authorizationService.GetUserBasicAuthorization(u)))
            .ToListAsync();

        return responseDto;
    }

    /// <summary>
    /// Get a list of users who have just joined (within 1 month from joining date).
    /// </summary>
    /// <returns>An object containing the users who have just joined.</returns>
    public async Task<UserListResponseDto> GetJoinedRecentlyListAsync()
    {
        DateOnly minimumJoiningDate = DateOnly
            .FromDateTime(DateTime.UtcNow.ToApplicationTime())
            .AddMonths(-1);

        List<User> users = await _context.Users
            .Include(u => u.Roles)
            .OrderBy(u => u.JoiningDate)
            .Where(u => u.JoiningDate.HasValue && !u.IsDeleted)
            .Where(u => u.JoiningDate.Value > minimumJoiningDate)
            .ToListAsync();

        return new UserListResponseDto
        {
            Results = users
                .Select(u => new UserBasicResponseDto(u))
                .ToList()
        };
    }

    /// <summary>
    /// Get a list of users who have upcoming birthday (within 1 month from now).
    /// </summary>
    /// <returns>An object containing the users who have upcoming birthday.</returns>
    public async Task<UserListResponseDto> GetUpcomingBirthdayListAsync()
    {
        DateOnly minRange = DateOnly.FromDateTime(DateTime.Today);
        DateOnly maxRange = minRange.AddMonths(1);
        int thisYear = DateTime.Today.Year;
        List<User> users = await _context.Users
            .Include(u => u.Roles)
            .OrderBy(u => u.Birthday.Value.Month).ThenBy(u => u.Birthday.Value.Day)
            .Where(u => u.Birthday.HasValue && !u.IsDeleted)
            .Where(u =>
                (
                    minRange <= u.Birthday.Value.AddYears(thisYear - u.Birthday.Value.Year) &&
                    maxRange > u.Birthday.Value.AddYears(thisYear - u.Birthday.Value.Year)
                ) || (
                    minRange <= u.Birthday.Value.AddYears(thisYear - u.Birthday.Value.Year + 1) &&
                    maxRange > u.Birthday.Value.AddYears(thisYear - u.Birthday.Value.Year + 1)
                )
            )
            .ToListAsync();

        return new UserListResponseDto
        {
            Results = users.Select(u => new UserBasicResponseDto(u)).ToList()
        };
    }

    /// <summary>
    /// Get the role information which is associated to the user with given id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>The full detail of the user's role.</returns>
    public async Task<RoleDetailResponseDto> GetRoleAsync(int id)
    {
        RoleDetailResponseDto responseDto = await _context.UserRoles
            .Include(ur => ur.Role).ThenInclude(r => r.Claims)
            .Where(ur => ur.UserId == id)
            .Select(ur => new RoleDetailResponseDto(ur.Role)).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(nameof(User), nameof(id), id.ToString());
        return responseDto;
    }

    /// <summary>
    /// Get fully detailed information, including role, claims (permissions)
    /// of the user with given id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>An object containing all details of the user.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with the given id cannot be found in the database.
    /// </exception>
    public async Task<UserDetailResponseDto> GetDetailAsync(int id)
    {
        User user = await _context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null)
        {
            throw new ResourceNotFoundException(nameof(User), nameof(id), id.ToString());
        }

        return new UserDetailResponseDto(
            user,
            _authorizationService.GetUserDetailAuthorization(user));
    }

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
    /// The user who sent the request doesn't have permission to create a new user.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Business rules violation during assign the new user to the specified role.
    /// </exception>
    public async Task<UserCreateResponseDto> CreateAsync(UserCreateRequestDto requestDto)
    {
        string fullName = PersonNameUtility.GetFullNameFromNameElements(
            requestDto.PersonalInformation.FirstName,
            requestDto.PersonalInformation.MiddleName,
            requestDto.PersonalInformation.LastName);

        // Create user.
        User user = new User
        {
            UserName = requestDto.UserName,
            FirstName = requestDto.PersonalInformation.FirstName,
            NormalizedFirstName = requestDto.PersonalInformation.FirstName
                .ToNonDiacritics()
                .ToUpper(),
            MiddleName = requestDto.PersonalInformation.MiddleName,
            NormalizedMiddleName = requestDto.PersonalInformation.MiddleName?
                .ToNonDiacritics()
                .ToUpper(),
            LastName = requestDto.PersonalInformation.LastName,
            NormalizedLastName = requestDto.PersonalInformation.LastName
                .ToNonDiacritics()
                .ToUpper(),
            FullName = fullName,
            NormalizedFullName = fullName.ToNonDiacritics().ToUpper(),
            Gender = requestDto.PersonalInformation.Gender,
            Birthday = requestDto.PersonalInformation.Birthday,
            PhoneNumber = requestDto.PersonalInformation.PhoneNumber,
            Email = requestDto.PersonalInformation.Email,
            JoiningDate = requestDto.UserInformation.JoiningDate,
            Note = requestDto.UserInformation.Note,
        };

        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        IdentityResult creatingUserResult;
        try
        {
            creatingUserResult = await _userManager.CreateAsync(
                user,
                requestDto.Password);
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException sqlException)
            {
                _exceptionHandler.Handle(sqlException);
                if (_exceptionHandler.IsUniqueConstraintViolated)
                {
                    throw new DuplicatedException(IdentifierCasingUtility
                        .SnakeCaseToPascalCase(_exceptionHandler.ViolatedFieldName));
                }
            }
            throw;
        }

        if (creatingUserResult.Succeeded)
        {
            // Checking if the role name from the request exist
            Role role = await _context.Roles
                .SingleOrDefaultAsync(r => r.Name == requestDto.UserInformation.Role.Name)
                ?? throw new ResourceNotFoundException(
                    nameof(Role),
                    nameof(requestDto.UserInformation.Role.Name),
                    requestDto.UserInformation.Role.Name);

            // Ensure the desired role's power level cannot be greater than
            // the requested user's power level.
            if (!_authorizationService.CanAssignToRole(role))
            {
                throw new AuthorizationException();
            }

            // Adding user to role
            IdentityResult result = await _userManager.AddToRoleAsync(
            user,
            requestDto.UserInformation.Role.Name);
            if (result.Errors.Any())
            {
                throw new InvalidOperationException(result.Errors
                    .Select(e => e.Description)
                    .First());
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Save avatar if included in the request.
            if (requestDto.PersonalInformation.AvatarFile != null)
            {
                user.AvatarUrl = await _photoService
                    .CreateAsync(requestDto.PersonalInformation.AvatarFile, "user", true);
            }

            return new UserCreateResponseDto { Id = user.Id };
        }

        if (creatingUserResult.Errors.Any(e => e.Code == "DuplicateUserName"))
        {
            throw new DuplicatedException(nameof(requestDto.UserName));
        }

        throw new InvalidOperationException(creatingUserResult.Errors
            .Select(error => error.Description)
            .First());
    }

    /// <summary>
    /// Update a user with given id.
    /// </summary>
    /// <param name="id">The id of the user to be updated.</param>
    /// <param name="requestDto">An object containing new data to be updated.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have enough permission to
    /// perform the updating action.
    /// </exception>
    public async Task UpdateAsync(int id, UserUpdateRequestDto requestDto)
    {
        // Fetch the entity from the database.
        User user = await _context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted)
                ?? throw new ResourceNotFoundException(
                    nameof(User),
                    nameof(id),
                    id.ToString());

        // Use transaction for atomic cancellation if there is any error during the operation.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Edit personal information if user has right.
        if (_authorizationService.CanEditUserPersonalInformation(user)
                && requestDto.PersonalInformation != null)
        {
            // Update avatar when the request specified.
            if (requestDto.PersonalInformation.AvatarChanged)
            {
                // Delete old avatar if there is any.
                if (user.AvatarUrl != null)
                {
                    _photoService.Delete(user.AvatarUrl);
                    user.AvatarUrl = null;
                }

                // Create new avatar if the request data contains it.
                if (requestDto.PersonalInformation.AvatarFile != null)
                {
                    string avatarUrl = await _photoService
                        .CreateAsync(requestDto.PersonalInformation.AvatarFile, "user", true);
                    user.AvatarUrl = avatarUrl;
                }
            }
            string fullName = PersonNameUtility.GetFullNameFromNameElements(
                requestDto.PersonalInformation.FirstName,
                requestDto.PersonalInformation.MiddleName,
                requestDto.PersonalInformation.LastName);
            user.FirstName = requestDto.PersonalInformation.FirstName;
            user.MiddleName = requestDto.PersonalInformation.MiddleName;
            user.LastName = requestDto.PersonalInformation.LastName;
            user.FullName = fullName;
            user.NormalizedFirstName = requestDto.PersonalInformation.LastName
                .ToNonDiacritics()
                .ToUpper();
            user.NormalizedMiddleName = requestDto.PersonalInformation.MiddleName?
                .ToNonDiacritics()
                .ToUpper();
            user.NormalizedLastName = requestDto.PersonalInformation.LastName
                .ToNonDiacritics()
                .ToUpper();
            user.NormalizedFullName = fullName.ToNonDiacritics().ToUpper();
            user.Gender = requestDto.PersonalInformation.Gender;
            user.Birthday = requestDto.PersonalInformation.Birthday;
            user.PhoneNumber = requestDto.PersonalInformation.PhoneNumber;
            user.Email = requestDto.PersonalInformation.Email;
            user.NormalizedEmail = requestDto.PersonalInformation.Email?
                .ToUpper();
        }
        else
        {
            throw new AuthorizationException();
        }

        // Edit user's user information if user has right.
        if (_authorizationService.CanEditUserUserInformation(user) &&
                requestDto.UserInformation != null)
        {
            user.JoiningDate = requestDto.UserInformation.JoiningDate;
            user.Note = requestDto.UserInformation.Note;

            // Update user's role if needed.
            if (requestDto.UserInformation.Role.Name != user.Role.Name)
            {
                // Ensure the desired role's power level cannot be greater than
                // the requested user's power level.
                Role role = await _context.Roles
                    .SingleOrDefaultAsync(r => r.Name == requestDto.UserInformation.Role.Name)
                    ?? throw new ResourceNotFoundException(
                        nameof(Role),
                        "role.name",
                        requestDto.UserInformation.Role.Name);
                if (!_authorizationService.CanAssignToRole(role))
                {
                    throw new AuthorizationException();
                }

                user.Roles.Remove(user.Role);
                user.Roles.Add(role);
            }
        }

        user.UpdatedDateTime = DateTime.UtcNow.ToApplicationTime();

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    
    /// <summary>
    /// Change the password of the user with given id.
    /// </summary>
    /// <param name="id">The id of the user to be changed his password.</param>
    /// <param name="requestDto">
    /// An object containing the current password, the new one and the confirmation one.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have enough permission to perform this action.
    /// </exception>
    /// <exception cref="OperationException">
    /// The current password is incorrect.
    /// </exception>
    public async Task ChangePasswordAsync(int id, [FromBody] UserPasswordChangeRequestDto requestDto)
    {
        // Fetch the entity with given id and ensure the entity exists.
        User user = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(User), nameof(id), id.ToString());

        // Ensure having permission to change password of the fetched user.
        if (!_authorizationService.CanChangeUserPassword(user))
        {
            throw new AuthorizationException();
        }

        // Performing password change operation.
        IdentityResult result = await _userManager
            .ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);

        // Ensure the operation succeeded.
        if (!result.Succeeded)
        {
            throw new OperationException(
                nameof(requestDto.CurrentPassword),
                result.Errors.First().Description);
        }
    }

    /// <summary>
    /// Reset the password of the user with given id (without the need of providing
    /// the current password).
    /// </summary>
    /// <param name="id">The id of the user to be reset password.</param>
    /// <param name="requestDto">
    /// An object containing new password and confirmation password.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to perform this action.
    /// </exception>
    /// <exception cref="OperationException">
    /// New password's complexity doesn't meet requirement.
    /// </exception>
    public async Task ResetPasswordAsync(int id, [FromBody] UserPasswordResetRequestDto requestDto)
    {
        // Fetch the entity with given id and ensure the entity exists.
        User user = await _context.Users
            .Include(u => u.Roles).ThenInclude(u => u.Claims)
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(User), nameof(id), id.ToString());

        // Check if having permission to reset password of the fetched user.
        if (!_authorizationService.CanResetUserPassword(user))
        {
            throw new AuthorizationException();
        }

        // Performing password reset operation.
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        IdentityResult result = await _userManager
            .ResetPasswordAsync(user, token, requestDto.NewPassword);

        // Ensure the operation succeeded.
        if (!result.Succeeded)
        {
            throw new OperationException(
                requestDto.NewPassword,
                result.Errors.First().Description);
        }
    }

    /// <summary>
    /// Delete the user with given id.
    /// </summary>
    /// <param name="id">The id of the user to be deleted.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user with given id cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to perform this action.
    /// </exception>
    public async Task DeleteAsync(int id)
    {
        // Fetch the user entity with given id from the database and ensure the entity exists.
        User user = await _context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(User),
                nameof(id),
                id.ToString());

        // Ensure having permission to delete the user.
        if (!_authorizationService.CanDeleteUser(user))
        {
            throw new AuthorizationException();
        }

        // Performing deleting operation.
        user.IsDeleted = true;

        // Save changes.
        await _context.SaveChangesAsync(); 
    }

    /// <summary>
    /// Restore the user with given id who has been marked as deleted.
    /// </summary>
    /// <param name="id">The id of the user to be restored.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The user who has given id and is marked as deleted cannot be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// The user who sent the request doesn't have permission to perform this action.
    /// </exception>
    public async Task RestoreAsync(int id)
    {
        // Fetch the user entity from the database and ensure the entity exists.
        User user = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id && u.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(User), nameof(id), id.ToString());

        // Ensure having permission to restore the user.
        if (!_authorizationService.CanRestoreUser(user))
        {
            throw new AuthorizationException();
        }

        // Perform restoration operation.
        user.IsDeleted = false;

        // Save changes.
        await _context.SaveChangesAsync();
    }
}