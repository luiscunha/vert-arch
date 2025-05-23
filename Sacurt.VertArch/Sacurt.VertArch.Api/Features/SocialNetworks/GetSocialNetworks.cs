using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sacurt.VertArch.Api.Common;
using Sacurt.VertArch.Api.Constants;
using Sacurt.VertArch.Api.Database;

namespace Sacurt.VertArch.Api.Features.SocialNetworks;

public static class GetSocialNetworks
{
    public record Query : IRequest<Result<List<Response>>>;

    public record Response(int Id, string Name);

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<List<Response>>>
    {
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var socialNetworks = await dbContext.SocialNetwork.AsNoTracking()
                .Select(s => new Response(s.Id, s.Name))
                .ToListAsync();

            return Result.Success(socialNetworks);
        }
    }
}

public sealed class GetSocialNetworksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.SocialNetworks.GetSocialNetworks, async (ISender sender, IMemoryCache cache) =>
        {
            var cachedSocialNetworks = await cache.GetOrCreateAsync("social-networks", async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return await sender.Send(new GetSocialNetworks.Query());
            });

            return Results.Ok(cachedSocialNetworks!.Value);
        });
    }
}
