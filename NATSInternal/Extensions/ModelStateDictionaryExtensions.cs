using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NATSInternal.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static void AddModelErrorsFromValidationErrors(
            this ModelStateDictionary modelState,
            List<ValidationFailure> validationFailures)
    {
        ClearModelState(modelState);
        foreach (ValidationFailure failure in validationFailures)
        {
            modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
        }
    }

    public static void AddModelErrorsFromServiceException(
            this ModelStateDictionary modelState,
            AuthenticationException exception)
    {
        ClearModelState(modelState);
        modelState.AddModelError("Password", exception.Message);
    }

    public static void AddModelErrorsFromServiceException(
            this ModelStateDictionary modelState,
            DuplicatedException exception)
    {
        ClearModelState(modelState);
        modelState.AddModelError(exception.PropertyName, exception.Message);
    }

    public static void AddModelErrorsFromServiceException(
            this ModelStateDictionary modelState,
            ResourceNotFoundException exception)
    {
        ClearModelState(modelState);
        modelState.AddModelError(exception.PropertyName ?? string.Empty, exception.Message);
    }

    public static void AddModelErrorsFromServiceException(
            this ModelStateDictionary modelState,
            OperationException exception)
    {
        ClearModelState(modelState);
        modelState.AddModelError(exception.PropertyName ?? string.Empty, exception.Message);
    }

    private static void ClearModelState(ModelStateDictionary modelState)
    {
        foreach (ModelStateEntry entry in modelState.Values)
        {
            entry.Errors.Clear();
        }
    }
}
