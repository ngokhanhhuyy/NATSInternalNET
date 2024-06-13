using NATSInternal.Services.Results;

namespace NATSInternal.Services.Results;

public class ServiceResult<TResponseDto>
{
    public TResponseDto ResponseDto { get; init; }
    public List<ServiceError> Errors { get; private set; } = new List<ServiceError>();
    public bool Succeeded => !Errors.Any();
    public bool HasValidationError => Errors.Any(e => e.ErrorType == ServiceErrorType.ValidationError);
    public bool HasNotFoundError => Errors.Any(e => e.ErrorType == ServiceErrorType.NotFoundError);
    public bool HasUndefinedError => Errors.Any(e => e.ErrorType == ServiceErrorType.UndefinedError);
    public bool HasOperationError => Errors.Any(e => e.ErrorType == ServiceErrorType.OperationError);
    public bool HasConcurrencyError => Errors.Any(e => e.ErrorType == ServiceErrorType.ConcurrencyError);

    public void AddError(string propertyName, string errorMessage)
    {
        Errors.Add(new ServiceError
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage
        });
    }

    public void AddError(ServiceError error)
    {
        Errors.Add(error);
    }

    public void AddErrors(params ServiceError[] errors)
    {
        Errors.AddRange(errors);
    }

    public static ServiceResult<TResponseDto> Success(TResponseDto responseDto)
    {
        return new ServiceResult<TResponseDto>
        {
            ResponseDto = responseDto
        };
    }

    public static ServiceResult<TResponseDto> Failed(List<ValidationFailure> failures)
    {
        return new ServiceResult<TResponseDto>
        {
            Errors = failures
                .Select(failure => new ServiceError
                {
                    ErrorType = ServiceErrorType.ValidationError,
                    PropertyName = failure.PropertyName,
                    ErrorMessage = failure.ErrorMessage
                }).ToList()
        };
    }

    public static ServiceResult<TResponseDto> Failed(ServiceError error)
    {
        return new ServiceResult<TResponseDto>
        {
            Errors = new List<ServiceError> { error }
        };
    }

    public static ServiceResult<TResponseDto> Failed(List<ServiceError> errors)
    {
        return new ServiceResult<TResponseDto>
        {
            Errors = errors
        };
    }

    public static ServiceResult<TResponseDto> Failed(
            ServiceErrorType errorType,
            string propertyName,
            string errorMessage)
    {
        return new ServiceResult<TResponseDto>
        {
            Errors = new List<ServiceError> {
                new ServiceError {
                    ErrorType = errorType,
                    PropertyName = propertyName,
                    ErrorMessage = errorMessage
                }
            }
        };
    }
}