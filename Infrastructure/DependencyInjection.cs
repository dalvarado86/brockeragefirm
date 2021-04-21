using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseSQLite"))
            {
                // Adding SQLite DbContext service
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(configuration.GetConnectionString("UseSQLiteConnection"));
                });
            }
            else
            {
                // Adding SQLServer DbContext service
                var connectionStringBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"))
                {
                    UserID = configuration["DbUser"],
                    Password = configuration["DbPassword"]
                };

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionStringBuilder.ConnectionString);
                });
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            // Adding Identity service
            var builder = services.AddIdentityCore<ApplicationUser>();
            var identiyBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identiyBuilder.AddEntityFrameworkStores<ApplicationDbContext>();
            identiyBuilder.AddSignInManager<SignInManager<ApplicationUser>>();
           
            services.AddAuthorization(options =>
            {
                // Adding a security policy: Only the account holder can sell/buy for his/her own account.
                options.AddPolicy("IsAccountHolder", policy =>
                {
                    policy.Requirements.Add(new IsAccountHolderRequeriment());
                });
            });

            services.AddTransient<IAuthorizationHandler, IsAccountHolderRequerimentHandler>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUserAccessor, UserAccesor>();

            return services;
        }
    }
}
