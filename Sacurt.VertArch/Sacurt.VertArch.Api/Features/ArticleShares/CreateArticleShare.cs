using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Database.Entities;
using Sacurt.VertArch.Api.Extensions;

namespace Sacurt.VertArch.Api.Features.ArticleShares;

public static class CreateArticleShare
{
    public record Command(int SocialNetworkId, string Comments) : IRequest<Result<Guid>>
    {
        public Guid ArticleId { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(IServiceProvider serviceProvider)
        {
            var scoped = serviceProvider.CreateScope();
            var dbContext = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            RuleFor(c => c.ArticleId).NotEmpty().DependentRules(() =>
                RuleFor(c => c.ArticleId).MustAsync(async (id, _) =>
                    await dbContext.Articles.AnyAsync(a => a.Id == id)
                ).WithMessage(command => $"The specified article id ({command.ArticleId}) doesn't exists.")
            );

            RuleFor(c => c.SocialNetworkId).NotEmpty().DependentRules(() =>
                RuleFor(c => c.SocialNetworkId).MustAsync(async (id, _) =>
                    await dbContext.SocialNetwork.AnyAsync(a => a.Id == id)
                ).WithMessage(command => $"The specified social network id ({command.SocialNetworkId}) doesn't exists.")
            );

            RuleFor(c => c.Comments).MaximumLength(500);
        }
    }

    internal sealed class Handler(ApplicationDbContext dbContext, IValidator<Command> validator) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(
                     new Error(ErrorType.Validation, "CreateArticleShare.Validation", validationResult.ToString())
                );
            }

            var articleShare = new ArticleShare()
            {
                ArticleId = command.ArticleId,
                SocialNetworkId = command.SocialNetworkId,
                SharedOnUtc = DateTime.UtcNow,
                Comments = command.Comments
            };

            await dbContext.ArticleShares.AddAsync(articleShare);
            await dbContext.SaveChangesAsync();

            return articleShare.Id;
        }
    }
}

public sealed class CreateArticleShareEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ArticleShares.CreateArticleShare, async (Guid articleId, CreateArticleShare.Command command, ISender sender) =>
        {
            command.ArticleId = articleId;

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return ApiResultsExtensions.ProblemDetails(result);
            }

            return Results.Ok(result.Value);
        })
        .WithName("CreateArticleShare")
        .Produces<Guid>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}
