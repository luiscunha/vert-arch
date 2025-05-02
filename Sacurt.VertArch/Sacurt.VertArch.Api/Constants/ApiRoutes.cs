using Sacurt.VertArch.Api.Entities;

namespace Sacurt.VertArch.Api.Constants;

internal static class ApiRoutes
{
    internal static class Articles
    {
        internal const string GetArticles = "api/articles";
        internal const string GetArticle = "api/articles/{id}";
        internal const string Create = "api/articles";
        internal const string PublishArticle = "api/articles/{id}/publish";
    }



}
