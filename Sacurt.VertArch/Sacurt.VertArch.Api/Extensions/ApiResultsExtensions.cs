using Microsoft.IdentityModel.Tokens;
using Sacurt.VertArch.Api.Common;

namespace Sacurt.VertArch.Api.Extensions;

internal static class ApiResultsExtensions
{
    public static IResult ProblemDetails(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Message,
            type: "",
            statusCode: GetStatusCode(result.Error.Type),
            extensions: null
        ); 

        static int GetStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Problem => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }

    }
}
