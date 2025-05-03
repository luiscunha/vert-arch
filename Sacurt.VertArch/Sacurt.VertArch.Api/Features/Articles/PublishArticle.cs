using Carter;
using MediatR;
using Sacurt.VertArch.Api.Authentication;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Extensions;
using Sacurt.VertArch.Api.Validation;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class PublishArticle
{
    public record Command(Guid Id) : IRequest<Result>;

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var article = await dbContext.Articles.FindAsync(request.Id);

            if(article is null)
            {
                return Result.Failure(ValidationErrors.Article.ArticleNotFound);
            }

            if (article.PublishedOnUtc.HasValue)
            {
                return Result.Failure(ValidationErrors.Article.ArticleAlreadyPublished);
            }

            article.PublishedOnUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
public sealed class PublishArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Articles.PublishArticle, async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new PublishArticle.Command(id));

            if (result.IsFailure)
            {
                return ApiResultsExtensions.ProblemDetails(result);
            }

            return Results.NoContent();
        })
           .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>(); 
    }
}
