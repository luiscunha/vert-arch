using Sacurt.VertArch.Api.Common;

namespace Sacurt.VertArch.Api.Validation;

internal static partial class ValidationErrors
{
    internal static class Article
    {
        internal static Error ArticleAlreadyPublished => new Error(ErrorType.Conflict, "Article.AlreadyPublished", "The article was already published.");

        internal static Error ArticleNotFound => new Error(ErrorType.NotFound, "Article.NotFound", "The specified article was not found.");
    }
}
