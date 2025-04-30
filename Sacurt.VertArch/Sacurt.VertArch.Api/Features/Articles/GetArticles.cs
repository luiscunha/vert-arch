using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Entities;
using System.Linq.Expressions;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class GetArticles
{
    public record Query(string? SearchTerm, string? SortColumn, string? SortOrder, int Page, int PageSize) : IRequest<Result<List<Response>>>;

    public record Response(Guid Id, string Title, string Content, DateTime CreatedOnUtc, DateTime? PublishedOnUtc, bool IsPublished, List<string> Tags);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<List<Response>>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken cancellationToken)
        {
            IQueryable<Article> articlesQuery = _dbContext.Articles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                articlesQuery = articlesQuery.Where(article =>
                    article.Title.Contains(query.SearchTerm) ||
                    article.Content.Contains(query.SearchTerm)
                    );
            }

            Expression<Func<Article, object>> keySelector = query.SortColumn?.ToLower() switch
            {
                "title" => article => article.Title,
                "content" => article => article.Content,
                "created" => article => article.CreatedOnUtc,
                _ => article => article.Id
            };

            if (query.SortOrder?.ToLower() == "desc")
            {
                articlesQuery = articlesQuery.OrderByDescending(keySelector);   
            }
            else
            {
                articlesQuery = articlesQuery.OrderBy(keySelector);
            }

            var articles = await articlesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(article => new Response(article.Id,
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
        app.MapGet("api/articles", async (
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

