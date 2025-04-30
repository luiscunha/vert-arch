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
        modelBuilder.Entity<Article>(builder =>
        {
            builder.Property(a => a.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                )
                .HasColumnType("nvarchar(max)"); // Or use "json" if your database supports it (e.g., PostgreSQL)
        });
    }

    public DbSet<Article> Articles { get; set; }    
}

