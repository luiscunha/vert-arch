using Microsoft.EntityFrameworkCore;
using Sacurt.VertArch.Api.Database.Entities;
using Sacurt.VertArch.Api.Entities;
using System.Text.Json;

namespace Sacurt.VertArch.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
          .Property(a => a.Title)
              .HasColumnType("varchar(100)");

        modelBuilder.Entity<Article>()
          .Property(a => a.Content)
              .HasColumnType("varchar(2000)");


        modelBuilder.Entity<ArticleShare>()
            .Property(a => a.Comments)
                .HasColumnType("varchar(500)");
         
        modelBuilder.Entity<SocialNetwork>().HasData(new List<SocialNetwork>()
        {
            new SocialNetwork{ Id = 1, Name = "Facebook" },
            new SocialNetwork{ Id = 2, Name = "Twitter / X"},
            new SocialNetwork{ Id = 3, Name = "Instagram"},
            new SocialNetwork{ Id = 4, Name = "LinkedIn"}
        });
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<ArticleShare> ArticleShares { get; set; }

    public DbSet<SocialNetwork> SocialNetwork { get; set; }
}

