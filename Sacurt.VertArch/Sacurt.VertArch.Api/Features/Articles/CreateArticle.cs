using Carter;
using FluentValidation;
using MediatR;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Entities;

namespace Sacurt.VertArch.Api.Features.Articles;

public static class CreateArticle
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

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
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IValidator<Command> _validator = validator;

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(
                    new Error("CreateArticle.Validation", validationResult.ToString())
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

            await _dbContext.AddAsync(article, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return article.Id;
        } 
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/articles", async (Command command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);    
                }

                return Results.Ok(result.Value);

            });
        }
    }
     

}

