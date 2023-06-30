using EECEBOT.API.Common.Errors.ErrorModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EECEBOT.API.Common.Errors;

public abstract class BaseUtility 
{
    protected static ObjectResult Problem(
        string invocationId,
        string? detail = null,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? errorCode = null)
    {
        var problemResultDetails = EecebotProblemResultFactory.CreateProblemResultDetails(
            statusCode: statusCode ?? 500,
            title: title,
            type: type,
            detail: detail,
            errorCode: errorCode,
            invocationId: invocationId);

        return new ProblemResult(problemResultDetails)
        {
            StatusCode = problemResultDetails.Status
        };
    }

    protected static ObjectResult ValidationProblem(ModelStateDictionary modelState, string invocationId)
    {
        var validationProblemDetails = EecebotProblemResultFactory
            .CreateValidationProblemDetails(modelState, invocationId: invocationId);
        
        return new ValidationProblemResult(validationProblemDetails);
    }
}