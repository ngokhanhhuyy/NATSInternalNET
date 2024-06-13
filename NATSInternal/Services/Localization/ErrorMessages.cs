namespace NATSInternal.Services.Localization;

public static class ErrorMessages
{
    // Error messages for validation
    public const string NotNull = "{PropertyName} không được để trống.";
    public const string NotEmpty = "{PropertyName} không được để trống.";
    public const string NotEqual = "{PropertyName} phải bằng {ComparisonValue}.";
    public const string MinimumLength = "{PropertyName} phải chứa tối thiểu {MinLength} ký tự.";
    public const string MaximumLength = "{PropertyName} chỉ được chứa tối đa {MaxLength} ký tự.";
    public const string StringLength = "{PropertyName} phải chứa trong khoảng từ {MinLength} đến {MaxLength} ký tự.";
    public const string CollectionMinimumLength = "{PropertyName} phải chứa tối thiểu {MinLength} phần tử.";
    public const string CollectionMaximumLength = "{PropertyName} chỉ được chứa tối đa {MaxLength} phần tử.";
    public const string CollectionLength = "{PropertyName} chỉ được chứa trong khoảng từ {MinLength} đến {MaxLength} phần tử.";
    public const string LessThan = "{PropertyName} phải có giá trị nhỏ hơn {ComparisonValue}.";
    public const string LessThanOrEqual = "{PropertyName} phải có giá trị nhỏ hơn {ComparisonValue}.";
    public const string GreaterThan = "{PropertyName} phải có giá trị lớn hơn {ComparisonValue}.";
    public const string GreaterThanOrEqual = "{PropertyName} phải có giá trị lớn hơn {ComparisonValue}.";
    public const string Invalid = "{PropertyName} không hợp lệ.";
    public const string EarlierThanOrEqualNow = "{PropertyName} phải là ngày giờ trước hoặc trùng với thời điểm hiện tại ({Now}).";
    public const string EarlierThanOrEqualToday = "{PropertyName} phải là ngày trước hoặc trùng với ngày hôm nay ({Today}).";
    public const string Null = "{PropertyName} phải chứa giá trị null.";

    // Error messages for business operations
    public const string Undefined = "Đã xảy ra lỗi không xác định.";
    public const string UniqueDuplicated = "{PropertyName} đã tồn tại.";
    public const string NotFound = "{ResourceName} không tồn tại.";
    public const string NotFoundByProperty = "{ResourceName} có {PropertyName} '{AttemptedValue}' không tồn tại.";
    public const string ImageFormatNotAllowed = "Hình ảnh có định dạng không được hỗ trợ.";
    public const string Incorrect = "{PropertyName} không chính xác";
    public const string MismatchedWith = "{PropertyName} không khớp với {ComparisonPropertyName}.";
    public const string InvalidUserNamePattern = "{PropertyName} chỉ được chứa chữ cái hoặc chữ số.";
    public const string NotAvailable = "{ResourceName} hiện đang không khả dụng.";
    public const string NotAvailableByProperty = "{ResourceName} có {PropertyName} '{AttemptedValue}' hiện đang không khả dụng.";
    public const string UpdateRestricted = "{ResourceName} không thể được chỉnh sửa do liên kết với các tài nguyên khác.";
    public const string DeleteRestricted = "{ResourceName} không thể được xoá do liên kết với các tài nguyên khác.";
}