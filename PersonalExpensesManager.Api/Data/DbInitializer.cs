using Microsoft.EntityFrameworkCore;
using PersonalExpensesManager.Api.Models;
using PersonalExpensesManager.Api.Services;

namespace PersonalExpensesManager.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var passwordService = services.GetRequiredService<PasswordService>();

        var hasUserTable = await HasTableAsync(db, "Users");
        if (!hasUserTable)
        {
            await db.Database.EnsureDeletedAsync();
        }

        await db.Database.EnsureCreatedAsync();

        if (await db.Users.AnyAsync())
        {
            return;
        }

        var demoUser = new User
        {
            FullName = "Демо користувач",
            Email = "demo@example.com",
            PasswordHash = passwordService.Hash("password123"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        UserSeedService.AddStarterData(demoUser);
        db.Users.Add(demoUser);
        await db.SaveChangesAsync();
    }

    private static async Task<bool> HasTableAsync(AppDbContext db, string tableName)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $tableName";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "$tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();
        await connection.CloseAsync();
        return Convert.ToInt32(result) > 0;
    }
}
