using FluentValidation;
using MediatR;
using MediatrBuilder.Configuration;
using MediatrBuilder.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatrBuilder;

public static class MediatrBuilderMiddleware
{
    public static IServiceCollection AddMediatrBuilder(this IServiceCollection services, Action<MediatrBuilderOptions> options)
    {
        var mediatrBuilderOptions = new MediatrBuilderOptions();
        options.Invoke(mediatrBuilderOptions);


        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatrBuilderOptions.AssembliesToRegister.ToArray()));

        if (!mediatrBuilderOptions.UseFluentValidation) return services;
        
        foreach (var assembly in mediatrBuilderOptions.AssembliesToRegister)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
            
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ApplicationPipelineBehaviour<,>));

        return services;
    }
}