using AutoMapper;
using ECommerce.Application.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Application;

 
public static class ConfigureRequestHandlerServices
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ECommerceProfile());
        });
        IMapper mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
        return services;

        
    }
}
 