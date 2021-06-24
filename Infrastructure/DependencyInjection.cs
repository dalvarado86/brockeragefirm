using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    /// <summary>
    /// DependencyInjection.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the infrastructure services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Adding DbContext service
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

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
