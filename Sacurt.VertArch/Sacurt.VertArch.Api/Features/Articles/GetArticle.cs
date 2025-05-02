using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Extensions;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class GetArticle
{
    public record Query(Guid Id) : IRequest<Result<Response>>;

    public record Response(Guid Id, string Title, string Content, DateTime CreatedOnUtc, DateTime? PublishedOnUtc, List<string> Tags);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var article = await _dbContext.Articles
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new Response(article.Id,
                    article.Title,
                    article.Content,
                    article.CreatedOnUtc,
                    article.PublishedOnUtc,
                    article.Tags
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (article == null)
            {
                return Result.Failure<Response>(new Error(ErrorType.BadRequest, "GetArticle.NotFound", "Article not found"));
            }

            return Result.Success(article);
        }
    }
}

public sealed class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Articles.GetArticle, async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetArticle.Query(id));

            if (result.IsFailure)
            {
                return ApiResultsExtensions.ProblemDetails(result);
            }

            return Results.Ok(result.Value);
        });
    }
}

