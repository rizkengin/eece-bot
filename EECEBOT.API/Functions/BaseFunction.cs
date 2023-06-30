using EECEBOT.API.Common.Errors;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EECEBOT.API.Functions;

public class BaseFunction : BaseUtility
{
    protected static IActionResult Problem(List<Error> errors, string invocationId)
    {
        if (errors.Count == 0)
        {
            return Problem(invocationId: invocationId);
        }

        if(errors.TrueForAll(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors, invocationId: invocationId);
        }

        var firstError = errors[0];

        return Problem(invocationId: invocationId, statusCode: firstError.NumericType, title: firstError.Description, errorCode: firstError.Code);
    }

    private static IActionResult ValidationProblem(List<Error> errors, string invocationId)
    {
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Description);
        }

        return ValidationProblem(modelStateDictionary, invocationId: invocationId);
    }
}