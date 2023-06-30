using EECEBOT.API.Common.Errors.Common;
using EECEBOT.API.Common.Errors.ErrorModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EECEBOT.API.Common.Errors;

public static class EecebotProblemResultFactory
{
    public static ProblemResultDetails CreateProblemResultDetails(
        string invocationId,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? errorCode = null)
    {
        statusCode ??= 500;

        var problemResultDetails = new ProblemResultDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            ErrorCode = errorCode,
            InvocationId = invocationId
        };

        ApplyProblemResultDefaults(problemResultDetails, statusCode.Value);

        return problemResultDetails;
    }

    public static ValidationProblemResultDetails CreateValidationProblemDetails(
        ModelStateDictionary modelStateDictionary,
        string invocationId,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        statusCode ??= 400;

        var problemResultDetails = new ValidationProblemResultDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            InvocationId = invocationId
        };

        if (title != null)
        {
            // For validation problem details, don't overwrite the default title with null.
            problemResultDetails.Title = title;
        }

        ApplyProblemResultDefaults(problemResultDetails, statusCode.Value);

        return problemResultDetails;
    }

    private static void ApplyProblemResultDefaults(ProblemResultDetails problemResult, int statusCode, string? errorCode = null)
    {
        problemResult.Status ??= statusCode;

        if (ClientData.ErrorMapping.TryGetValue(statusCode, out var clientErrorData))
        {
            problemResult.Type ??= clientErrorData;
            problemResult.Title ??= "Unexpected error occurred.";
        }

        if (errorCode is not null)
        {
            problemResult.ErrorCode = errorCode;
        }
    }
}