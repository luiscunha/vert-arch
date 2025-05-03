
namespace Sacurt.VertArch.Api.Authentication;

internal class ApiKeyAuthenticationEndpointFilter(IConfiguration configuration) : IEndpointFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

        if (IsApiKeyInvalid(apiKey))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }

    private bool IsApiKeyInvalid(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return true;
        }

        var configuredApiKey = configuration.GetValue<string>("ApiKeySecret")!;

        return apiKey != configuredApiKey;

    }
}
