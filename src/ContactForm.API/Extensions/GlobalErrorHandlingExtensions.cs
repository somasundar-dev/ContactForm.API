using ContactForm.API.Middleware;

namespace ContactForm.API.Extensions;

public static class GlobalErrorHandlingExtensions
{
    public static IApplicationBuilder useGlobalErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }

    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddTransient<ErrorHandlingMiddleware>();
        return services;
    }
}
