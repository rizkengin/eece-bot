using Microsoft.AspNetCore.Mvc;

namespace EECEBOT.API.Common.Errors.ErrorModels;

public class ValidationProblemResult : ProblemResult
{
    public ValidationProblemResult(ValidationProblemResultDetails validationProblemResultDetails) : base(validationProblemResultDetails)
    {
    }

    public override void OnFormatting(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (Value is ValidationProblemResultDetails details)
        {
            if (details.Status != null && StatusCode == null)
            {
                StatusCode = details.Status;
            }
            else if (details.Status == null && StatusCode != null)
            {
                details.Status = StatusCode;
            }
        }

        if (StatusCode.HasValue)
        {
            context.HttpContext.Response.StatusCode = StatusCode.Value;
        }
    }
}