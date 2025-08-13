using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace ECommerce.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContext<ECommerceDbContext>(options =>
        //options.UseSqlServer(configuration.GetConnectionString("ECommerceConnection")));

        services.AddDbContext<ECommerceDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("ECommerceConnection")));


        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserInRoleRepository, UserInRoleRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }


}
