using System.Threading.RateLimiting;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.SimpleEmailV2;
using Asp.Versioning;
using ContactForm.API.Constants;
using ContactForm.API.Extensions;
using ContactForm.API.Interfaces;
using ContactForm.API.Services;
using FastEndpoints;
using FastEndpoints.AspVersioning;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGlobalErrorHandling();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddKeyedScoped<IEmailService, SMTPEmailService>("SMTP");
builder.Services.AddKeyedScoped<IEmailService, SESService>("AWSSES");

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleEmailServiceV2>();

/// ContactForm.API::ContactForm.API.LambdaEntryPoint::FunctionHandlerAsync
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

VersionSets.CreateApi("Contact Form API", v => v.HasApiVersion(new ApiVersion(1.0)).HasApiVersion(new ApiVersion(2.0)));
builder.Services.AddFastEndpoints();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();
builder.Services.ConfigureAppCors(builder.Configuration);
builder.Services.ConfigureAppOptions(builder.Configuration);
builder.Services.AddVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
});
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, cancellationToken) =>
    {
        // Custom rejection handling logic
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        await context.HttpContext.Response.WriteAsync(
            "Rate limit exceeded. Please try again later.",
            cancellationToken
        );
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", context.HttpContext.Connection.RemoteIpAddress);
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }
        )
    );
});

var app = builder.Build();

app.useGlobalErrorHandling();

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseRateLimiter();

app.UseCors(AppConstant.CorsPolicyName);

app.MapGet("/", () => "Contact Form API is running!").WithName("Contact Form API");

app.MapHealthChecks("/health");

app.UseFastEndpoints(c =>
{
    // c.Endpoints.Configurator = ep =>
    // {
    //     ep.PreProcessor<ApiVersionSecurityProcessor>(Order.Before);
    // };
});

app.Run();
