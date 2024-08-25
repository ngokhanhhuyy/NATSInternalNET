using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NATSInternal.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;

    public AuthenticationService(
            DatabaseContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _tokenHandler = new JwtSecurityTokenHandler();

        SymmetricSecurityKey securityKey;
        securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        _signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    public async Task<AccessTokenResponseDto> GetAccessTokenAsync(SignInRequestDto requestDto)
    {
        // Check if user exists.
        User user = _userManager.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .Include(u => u.RefreshTokens)
            .SingleOrDefault(u => u.UserName == requestDto.UserName && !u.IsDeleted);
        if (user == null)
        {
            throw new OperationException(
                nameof(requestDto.UserName),
                ErrorMessages.NotFound.ReplaceResourceName(DisplayNames.UserName));
        }

        // Check if password is correct.
        bool passwordValid = await _userManager.CheckPasswordAsync(user, requestDto.Password);
        if (!passwordValid)
        {
            throw new OperationException(
                nameof(requestDto.Password),
                ErrorMessages.Incorrect
                    .ReplacePropertyName(DisplayNames.Password));
        }

        // Generate refresh token.
        string refreshToken = GenerateRefreshToken();
        DateTime refreshTokenIssuedDateTime = DateTime.UtcNow.ToApplicationTime();

        // Store the refresh token in the database.
        user.RefreshTokens.Add(new UserRefreshToken
        {
            Token = refreshToken,
            IssuedDateTime = refreshTokenIssuedDateTime,
            ExpiringDateTime = refreshTokenIssuedDateTime.AddDays(7)
        });
        await _context.SaveChangesAsync();

        DateTime expiringDateTime = DateTime.UtcNow.ToApplicationTime().AddDays(7);
        return new AccessTokenResponseDto
        {
            AccessToken = GenerateAccessToken(user, expiringDateTime),
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiringDateTime = expiringDateTime
        };
    }

    public async Task<AccessTokenResponseDto> ExchangeAccessTokenAsync(
            AccessTokenExchangeRequestDto requestDto)
    {
        // Validate the access token and extract the user id stored in the token.
        (int userId, string userName) userIdentity;
        userIdentity = ExtractUserIdentityFromAccessToken(requestDto.ExpiredAccessToken);

        // Verifying the user identity and the refresh token associated with the user.
        User user = await _context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .Include(u => u.RefreshTokens)
            .SingleAsync(u => u.Id == userIdentity.userId)
            ?? throw new OperationException("Tài khoản không tồn tại.");

        UserRefreshToken storedRefreshToken = user.RefreshTokens
            .FirstOrDefault(urt => urt.Token == requestDto.RefreshToken)
            ?? throw new OperationException("Mã làm mới không tồn tại.");

        // Check if the refresh token has expired.
        if (storedRefreshToken.ExpiringDateTime < DateTime.UtcNow.ToApplicationTime())
        {
            throw new OperationException("Mã làm mới đã hết hạn.");
        }

        // Refresh token is valid, generate another one.
        storedRefreshToken.Token = GenerateRefreshToken();
        storedRefreshToken.IssuedDateTime = DateTime.UtcNow.ToApplicationTime();
        storedRefreshToken.ExpiringDateTime = storedRefreshToken.IssuedDateTime.AddDays(7);

        // Save new refresh token into the database.
        await _context.SaveChangesAsync();

        DateTime expiringDateTime = DateTime.UtcNow.ToApplicationTime().AddMinutes(30);
        AccessTokenResponseDto responseDto = new AccessTokenResponseDto
        {
            AccessToken = GenerateAccessToken(user, expiringDateTime),
            UserId = user.Id,
            RefreshToken = storedRefreshToken.Token,
            ExpiringDateTime = expiringDateTime
        };

        return responseDto;
    }

    public async Task SignInAsync(SignInRequestDto requestDto)
    {
        // Check if user exists
        int userId = await _userManager.Users
            .Where(u => u.UserName == requestDto.UserName)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();
        if (userId == 0)
        {
            throw new ResourceNotFoundException(
                nameof(User),
                nameof(requestDto),
                requestDto.UserName);
        }

        // Performing login
        Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
            .PasswordSignInAsync(
                userName: requestDto.UserName,
                password: requestDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            throw new OperationException(
                nameof(requestDto.Password),
                ErrorMessages.Incorrect
                    .Replace("{PropertyName}", nameof(requestDto.Password)));
        }
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private string GenerateAccessToken(User user, DateTime expiringDateTime) {
        // Prepare payload for access token.
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, user.Role.Name!),
            .. user.Role.Claims.Select(c => new Claim("Permission", c.ClaimValue!))
        ];

        // Generate access token.
        JwtSecurityToken accessToken = new JwtSecurityToken(
            _config["JwtSettings:Issuer"],
            _config["JwtSettings:Issuer"],
            claims,
            expires: expiringDateTime,
            signingCredentials: _signingCredentials);
        return _tokenHandler.WriteToken(accessToken);
    }

    private string GenerateRefreshToken()
    {
        const int tokenLength = 64;
        Random random = new Random();
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string token = string.Empty;
        for (int i = 0; i < tokenLength; i++)
        {
            token += characters[random.Next(characters.Length)];
        }

        return token;
    }

    private (int, string) ExtractUserIdentityFromAccessToken(string accessToken)
    {
        TokenValidationParameters validationParameters;
        validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["JwtSettings:Issuer"],
            ValidAudience = _config["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]))
        };

        ClaimsPrincipal claimsPrincipal;
        try
        {
            claimsPrincipal = _tokenHandler.ValidateToken(
                accessToken,
                validationParameters,
                out SecurityToken token);
        }
        catch (Exception)
        {
            throw new OperationException("Mã truy cập không hợp lệ"); ;
        }

        Claim[] claims = claimsPrincipal.Claims.ToArray();
        // Verifying user id in the payload.
        string userIdAsString = claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? throw new OperationException("ID người dùng không được chứa trong mã truy cập.");;
        bool parsable = int.TryParse(userIdAsString, out int userId);
        if (!parsable)
        {
            throw new OperationException("ID người dùng không hợp lệ.");
        }

        string userName = claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? throw new OperationException("Tên tài khoản người dùng không hợp lệ."); ;

        return (userId, userName);
    }
}
