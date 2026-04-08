using Daira.Application.Interfaces;
using Daira.Application.Interfaces.Auth;
using Daira.Application.Interfaces.CommentModule;
using Daira.Application.Interfaces.PostModule;
using Daira.Infrastructure.Persistence;
using Daira.Infrastructure.Persistence.DbContext;
using Daira.Infrastructure.Services.AuthService;
using Daira.Infrastructure.Services.CommentService;
using Daira.Infrastructure.Services.PostService;
using Daira.Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Daira.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DairaDbContext>(options =>
           options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection"),
               b => b.MigrationsAssembly(typeof(DairaDbContext).Assembly.FullName)));

            // Configure ASP.NET Core Identity with ApplicationRole
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<DairaDbContext>()
            .AddDefaultTokenProviders();

            // Configure JWT settings
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            // Configure JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Configure Authorization Policies
            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                .AddPolicy("ModeratorOrAdmin", policy => policy.RequireRole("Admin", "Moderator"));

            // Register Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();

            // Configure email settings
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            return services;
        }
    }
}
