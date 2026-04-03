using Daira.Infrastructure.Persistence.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Daira.Infrastructure.Persistence.SeedDataClass
{
    public static class SeedDataClass
    {
        public static async Task SeedDataAsync(this DairaDbContext context, string contentRootPath, ILogger? logger = null)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true
            };

            var passwordHasher = new PasswordHasher<AppUser>();

            // Find seed files path
            var seedFilesPath = GetSeedFilesPath(contentRootPath);
            if (!Directory.Exists(seedFilesPath))
            {
                logger?.LogError("Seed files directory not found at: {Path}", seedFilesPath);
                throw new DirectoryNotFoundException($"Seed files not found at: {seedFilesPath}");
            }

            logger?.LogInformation("Starting database seeding from: {Path}", seedFilesPath);

            // Check if database is already seeded
            if (await IsDatabaseSeeded(context))
            {
                logger?.LogInformation("Database is already seeded. Skipping seed process.");
                return;
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Seed Roles
                await SeedRolesAsync(context, seedFilesPath, options, logger);

                // Seed Users
                await SeedUsersAsync(context, seedFilesPath, options, passwordHasher, logger);

                // Seed UserRoles
                await SeedUserRolesAsync(context, seedFilesPath, options, logger);



                // Seed RefreshTokens
                await SeedRefreshTokensAsync(context, seedFilesPath, options, logger);

                await transaction.CommitAsync();
                logger?.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger?.LogError(ex, "Error occurred while seeding database. Rolling back transaction.");
                throw;
            }
        }

        private static string GetSeedFilesPath(string contentRootPath)
        {
            var seedFilesPath = Path.Combine(contentRootPath, "Persistence", "SeedFiles");

            if (!Directory.Exists(seedFilesPath))
            {
                seedFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Daira.Infrastructure", "Persistence", "SeedFiles");
            }

            if (!Directory.Exists(seedFilesPath))
            {
                seedFilesPath = Path.Combine(AppContext.BaseDirectory, "SeedFiles");
            }

            return seedFilesPath;
        }

        private static async Task<bool> IsDatabaseSeeded(DairaDbContext context)
        {
            return await context.Roles.AnyAsync() || await context.Users.AnyAsync();
        }

        private static async Task SeedRolesAsync(DairaDbContext context, string seedFilesPath, JsonSerializerOptions options, ILogger? logger)
        {
            if (await context.Roles.AnyAsync())
            {
                logger?.LogInformation("Roles already exist. Skipping.");
                return;
            }

            var filePath = Path.Combine(seedFilesPath, "AppRole.json");
            if (!File.Exists(filePath))
            {
                logger?.LogWarning("Roles.json file not found at: {Path}", filePath);
                return;
            }

            var jsonData = await File.ReadAllTextAsync(filePath);
            var roles = JsonSerializer.Deserialize<List<AppRole>>(jsonData, options);

            if (roles == null || !roles.Any())
            {
                logger?.LogWarning("No roles found in Roles.json");
                return;
            }

            foreach (var role in roles)
            {
                role.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} roles", roles.Count);
        }

        private static async Task SeedUsersAsync(DairaDbContext context, string seedFilesPath, JsonSerializerOptions options, PasswordHasher<AppUser> passwordHasher, ILogger? logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger?.LogInformation("Users already exist. Skipping.");
                return;
            }

            var filePath = Path.Combine(seedFilesPath, "Appuser.json");
            if (!File.Exists(filePath))
            {
                logger?.LogWarning("Users.json file not found at: {Path}", filePath);
                return;
            }

            var jsonData = await File.ReadAllTextAsync(filePath);
            var users = JsonSerializer.Deserialize<List<AppUser>>(jsonData, options);

            if (users == null || !users.Any())
            {
                logger?.LogWarning("No users found in Users.json");
                return;
            }

            foreach (var user in users)
            {
                user.NormalizedUserName = user.UserName?.ToUpper();
                user.NormalizedEmail = user.Email?.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                user.PasswordHash = passwordHasher.HashPassword(user, "P@ssw0rd");
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} users", users.Count);
        }

        private static async Task SeedUserRolesAsync(DairaDbContext context, string seedFilesPath, JsonSerializerOptions options, ILogger? logger)
        {
            if (await context.UserRoles.AnyAsync())
            {
                logger?.LogInformation("UserRoles already exist. Skipping.");
                return;
            }

            var filePath = Path.Combine(seedFilesPath, "UserRoles.json");
            if (!File.Exists(filePath))
            {
                logger?.LogWarning("UserRoles.json file not found at: {Path}", filePath);
                return;
            }

            var jsonData = await File.ReadAllTextAsync(filePath);
            var userRolesMappings = JsonSerializer.Deserialize<List<UserRoleMapping>>(jsonData, options);

            if (userRolesMappings == null || !userRolesMappings.Any())
            {
                logger?.LogWarning("No user role mappings found in UserRoles.json");
                return;
            }

            var userRoles = new List<IdentityUserRole<string>>();

            foreach (var mapping in userRolesMappings)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == mapping.UserEmail);
                var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == mapping.RoleName);

                if (user != null && role != null)
                {
                    userRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    });
                }
                else
                {
                    logger?.LogWarning("Could not create user role mapping for {Email} - {Role}", mapping.UserEmail, mapping.RoleName);
                }
            }

            if (userRoles.Any())
            {
                await context.UserRoles.AddRangeAsync(userRoles);
                await context.SaveChangesAsync();
                logger?.LogInformation("Seeded {Count} user role mappings", userRoles.Count);
            }
        }

        private static async Task SeedRefreshTokensAsync(DairaDbContext context, string seedFilesPath, JsonSerializerOptions options, ILogger? logger)
        {
            if (await context.RefreshTokens.AnyAsync())
            {
                logger?.LogInformation("RefreshTokens already exist. Skipping.");
                return;
            }

            var filePath = Path.Combine(seedFilesPath, "RefreshTokens.json");
            if (!File.Exists(filePath)) return;

            var jsonData = await File.ReadAllTextAsync(filePath);
            var refreshTokens = JsonSerializer.Deserialize<List<RefreshToken>>(jsonData, options);

            if (refreshTokens == null || !refreshTokens.Any()) return;

            await context.RefreshTokens.AddRangeAsync(refreshTokens);
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} refresh tokens", refreshTokens.Count);
        }
    }
    public class UserRoleMapping
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
