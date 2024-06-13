namespace NATSInternal.Services.Identity;

public class VietnameseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
    {
        return new IdentityError
        {
            Code = nameof(DefaultError),
            Description = "Đã có lỗi không xác định xảy ra."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = "Email đã tồn tại.",
        };
    }

    public override IdentityError DuplicateRoleName(string roleName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = "Vị trí đã tồn tại.",
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = "Tên tài khoản đã tồn tại.",
        };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = "Email không hợp lệ.",
        };
    }

    public override IdentityError InvalidRoleName(string roleName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidRoleName),
            Description = "Tên vị trí không hợp lệ.",
        };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = "Tên tài khoản không hợp lệ.",
        };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError
        {
            Code = nameof(PasswordMismatch),
            Description = "Mật khẩu không chính xác.",
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Mật khẩu phải chứa chữ số ('0'-'9')."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Mật khẩu phải chứa chữ cái viết thường ('a'-'z')."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Mật khẩu phải chứa ký tự không phải chữ cái và chữ số ('a'-'z', 'A'-'Z', '0'-'9').",
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"Mật khẩu phải chứa ít nhất {uniqueChars} ký tự không trùng với các ký tự còn lại."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Mật khẩu phải chứa chữ cái viết hoa ('A'-'Z')."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Mật khẩu phải có độ dài ít nhất {length} ký tự."
        };
    }

    public override IdentityError UserAlreadyInRole(string roleName)
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"Tài khoản đã ở trong vị trí {roleName} từ trước đó.",
        };
    }

    public override IdentityError UserNotInRole(string roleName)
    {
        return new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = $"Tài khoản hiện không ở trong vị trí {roleName}."
        };
    }
}