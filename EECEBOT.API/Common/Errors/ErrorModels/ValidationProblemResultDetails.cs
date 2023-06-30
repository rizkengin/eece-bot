using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EECEBOT.API.Common.Errors.ErrorModels;

public class ValidationProblemResultDetails : ProblemResultDetails
{
    public ValidationProblemResultDetails(ModelStateDictionary modelState)
    {
        var errors = CreateErrorDictionary(modelState);
        
        Title = "One or more validation errors occurred.";
        
        Errors = errors;
    }

    private static IDictionary<string, string[]> CreateErrorDictionary(ModelStateDictionary modelState)
    {
        if (modelState == null)
        {
            throw new ArgumentNullException(nameof(modelState));
        }

        var errorDictionary = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var keyModelStatePair in modelState)
        {
            var key = keyModelStatePair.Key;
            var errors = keyModelStatePair.Value.Errors;
            if (errors.Count > 0)
            {
                if (errors.Count == 1)
                {
                    var errorMessage = GetErrorMessage(errors[0]);
                    errorDictionary.Add(key, new[] { errorMessage });
                }
                else
                {
                    var errorMessages = new string[errors.Count];
                    for (var i = 0; i < errors.Count; i++)
                    {
                        errorMessages[i] = GetErrorMessage(errors[i]);
                    }

                    errorDictionary.Add(key, errorMessages);
                }
            }
        }

        return errorDictionary;

        static string GetErrorMessage(ModelError error)
        {
            return error.ErrorMessage;
        }
    }
}