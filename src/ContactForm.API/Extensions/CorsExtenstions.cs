using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactForm.API.Constants;
using ContactForm.API.Models;

namespace ContactForm.API.Extensions;

public static class CorsExtenstions
{
    public static IServiceCollection ConfigureAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        CorsOptions corsOptions = new();
        configuration.GetSection("cors").Bind(corsOptions);
        services.AddCors(options =>
        {
            options.AddPolicy(
                AppConstant.CorsPolicyName,
                policy =>
                {
                    if (!string.IsNullOrWhiteSpace(corsOptions.Origins))
                    {
                        var Origins = corsOptions.Origins.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        policy.WithOrigins(Origins);
                    }
                    else
                    {
                        policy.AllowAnyOrigin();
                    }

                    if (!string.IsNullOrWhiteSpace(corsOptions.Methods))
                    {
                        var Methods = corsOptions.Methods.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        policy.WithMethods(Methods);
                    }
                    else
                    {
                        policy.AllowAnyMethod();
                    }

                    if (!string.IsNullOrWhiteSpace(corsOptions.Headers))
                    {
                        var Headers = corsOptions.Headers.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        policy.WithHeaders(Headers);
                    }
                    else
                    {
                        policy.AllowAnyHeader();
                    }

                    if (!string.IsNullOrWhiteSpace(corsOptions.ExposedHeaders))
                    {
                        var ExposedHeaders = corsOptions.ExposedHeaders.Split(
                            ",",
                            StringSplitOptions.RemoveEmptyEntries
                        );
                        policy.WithExposedHeaders(ExposedHeaders);
                    }

                    if (corsOptions != null && corsOptions.AllowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                    else
                    {
                        policy.DisallowCredentials();
                    }
                }
            );
        });

        return services;
    }
}
