namespace Sacurt.VertArch.Api.Database.Entities
{
    public class ArticleShare
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public SocialNetwork SocialNetwork { get; set; }
        public DateTime SharedOnUtc { get; set; }
        public string? Comments { get; set; }
    }
}
