using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Database;

namespace Sacurt.VertArch.Api.Features.Articles
{
    public static class GetArticle
    {
        public class  Query : IRequest<Result<GetArticleResponse>>
        {
            public Guid Id { get; set; }
        }

        public class GetArticleResponse()
        {
            public Guid Id { get; init; }

            public string Title { get; init; } 

            public string Content { get; init; } 

            public DateTime CreatedOnUtc { get; init; }

            public DateTime? PublishedOnUtc { get; init; }

            public List<string> Tags { get; init; }  
        }

        internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<GetArticleResponse>>
        {
            private readonly ApplicationDbContext _dbContext = dbContext;
             
            public async Task<Result<GetArticleResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var article =  await _dbContext.Articles
                    .Where(article => article.Id == request.Id)
                    .Select(article => new GetArticleResponse
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Content = article.Content,
                        CreatedOnUtc = article.CreatedOnUtc,
                        PublishedOnUtc = article.PublishedOnUtc,
                        Tags = article.Tags
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if(article == null)
                {
                    return Result.Failure<GetArticleResponse>(new Error("GetArticle.NotFound", "Article not found"));
                }

                return Result.Success(article);
            }
        }
    }

    public class GetArticleEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetArticle.Query { Id = id });

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }
}
