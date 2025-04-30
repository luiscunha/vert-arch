using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Database;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class GetArticles
{
    public record Query : IRequest<Result<List<GetArticlesResponse>>>;
    public record GetArticlesResponse(Guid Id, string Title, string Content, DateTime CreatedOnUtc, DateTime? PublishedOnUtc, bool IsPublished, List<string> Tags);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<List<GetArticlesResponse>>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<List<GetArticlesResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var articles = await _dbContext.Articles
                //.AsNoTracking()
                .Select(article => new GetArticlesResponse(article.Id,
                    article.Title,
                    article.Content,
                    article.CreatedOnUtc,
                    article.PublishedOnUtc,
                    article.PublishedOnUtc.HasValue,
                    article.Tags
                ))
                .ToListAsync(cancellationToken);

            return Result.Success(articles);
        }
    }
}

public sealed class GetArticlesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/articles", async (ISender sender) =>
        {
            var result = await sender.Send(new GetArticles.Query());

            return Results.Ok(result.Value);
        });
    }
}

