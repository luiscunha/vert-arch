﻿using Microsoft.EntityFrameworkCore;
using Sacurt.VertArch.Api.Database;

namespace Sacurt.VertArch.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}