using Carter;
using FluentValidation;
using MediatR;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Entities;
using Sacurt.VertArch.Api.Extensions;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class CreateArticle
{
    public record Command(string Title, string Content, List<string> Tags) : IRequest<Result<Guid>>; 

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();

            //RuleFor(x => x.Tags).NotEmpty().WithMessage("At least one tag is required.");
        }
    }

    internal sealed class Handler(ApplicationDbContext dbContext, IValidator<Command> validator) 
        : IRequestHandler<Command, Result<Guid>>
    { 
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(
                    new Error(ErrorType.Validation, "CreateArticle.Validation", validationResult.ToString())
                );
            }

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                CreatedOnUtc = DateTime.UtcNow
            };

            await dbContext.AddAsync(article, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return article.Id;
        } 
    }  
}

public sealed class CreateArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Articles.Create, async (CreateArticle.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return ApiResultsExtensions.ProblemDetails(result);
            }

            return Results.Ok(result.Value);

        });
    }
}