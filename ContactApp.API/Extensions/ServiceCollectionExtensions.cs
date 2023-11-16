﻿using ContactApp.API.Data;
using ContactApp.API.Repositories;
using ContactApp.API.Services;
using ContactApp.API.Repositories;
using ContactApp.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ContactApp.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string AllowedOriginSetting = "AllowedOrigin";

        public static IServiceCollection AddCorsSettings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.AddCors(options =>
            {
                options.AddDefaultPolicy(corsBuilder =>
                {
                    var allowedOrigin =
                        configuration[AllowedOriginSetting]
                        ?? throw new InvalidOperationException("AllowedOrigin is not set");
                    corsBuilder.WithOrigins(allowedOrigin).AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactService>();
            services.AddScoped<ICategoryRepository, CategoryService>();
            services.AddScoped<ISubcategoryRepository, SubcategoryService>();
            services.AddTransient<IAuthRepository, AuthService>();

            return services;
        }

        public static IServiceCollection AddDbContexts(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connString = configuration.GetConnectionString("ContactAppConnectionString");

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connString)
            );
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connString));

            return services;
        }

        public static void AddContactAppIdentity(this IServiceCollection services)
        {
            services
                .AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("ContactApp")
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
        }

        public static void AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        AuthenticationType = "Jwt",
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        )
                    };
                });
        }

        public static async Task InitializeDBAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await authDbContext.Database.MigrateAsync();
        }
    }
}
