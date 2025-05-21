using Microsoft.EntityFrameworkCore;
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
       // modelBuilder.Entity<Article>().OwnsOne(p => p.Tags, navBuilder => navBuilder.ToJson());
    }

    public DbSet<Article> Articles { get; set; }    
}

