using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Database;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class GetArticle
{
    public record Query(Guid Id) : IRequest<Result<GetArticleResponse>>;

    public record GetArticleResponse(Guid Id, string Title, string Content, DateTime CreatedOnUtc, DateTime? PublishedOnUtc, List<string> Tags);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<GetArticleResponse>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<GetArticleResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var article = await _dbContext.Articles
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new GetArticleResponse(article.Id,
                    article.Title,
                    article.Content,
                    article.CreatedOnUtc,
                    article.PublishedOnUtc,
                    article.Tags
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (article == null)
            {
                return Result.Failure<GetArticleResponse>(new Error("GetArticle.NotFound", "Article not found"));
            }

            return Result.Success(article);
        }
    }
}

public sealed class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetArticle.Query(id));

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}

