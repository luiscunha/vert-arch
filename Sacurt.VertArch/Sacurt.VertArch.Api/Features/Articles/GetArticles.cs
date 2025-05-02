using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Entities;
using System.Linq.Expressions;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class GetArticles
{
    public record Query(string? SearchTerm, string? SortColumn, string? SortOrder, int Page, int PageSize) : IRequest<Result<PagedList<Response>>>;

    public record Response(Guid Id, string Title, string Content, DateTime CreatedOnUtc, DateTime? PublishedOnUtc, bool IsPublished, List<string> Tags);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<PagedList<Response>>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<PagedList<Response>>> Handle(Query query, CancellationToken cancellationToken)
        {
            IQueryable<Article> articlesQuery = _dbContext.Articles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                articlesQuery = articlesQuery.Where(article =>
                    article.Title.Contains(query.SearchTerm) ||
                    article.Content.Contains(query.SearchTerm)
                );
            }

            if (query.SortOrder?.ToLower() == "desc")
            {
                articlesQuery = articlesQuery.OrderByDescending(GetKeyPropertySelector(query));
            }
            else
            {
                articlesQuery = articlesQuery.OrderBy(GetKeyPropertySelector(query));
            }

            var articlesResponsesQuery = articlesQuery
                .Select(article => new Response(article.Id,
                    article.Title,
                    article.Content,
                    article.CreatedOnUtc,
                    article.PublishedOnUtc,
                    article.PublishedOnUtc.HasValue,
                    article.Tags
                ));

            var articles = await PagedList<Response>.CreateAsync(articlesResponsesQuery, query.Page, query.PageSize, cancellationToken);

            return Result.Success(articles);
        }

        private static Expression<Func<Article, object>> GetKeyPropertySelector(Query query)
            => query.SortColumn?.ToLower() switch
            {
                "title" => article => article.Title,
                "content" => article => article.Content,
                "created" => article => article.CreatedOnUtc,
                _ => article => article.Id
            };
    }
}

public sealed class GetArticlesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Articles.GetArticles, async (
            string? searchTerm,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetArticles.Query(searchTerm, sortColumn, sortOrder, page, pageSize));

            return Results.Ok(result.Value);
        });
    }
}

