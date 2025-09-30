using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;
using UserManagement.Services.Validators;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IUserValidator, UserValidator>();

        return services;
    }
}
