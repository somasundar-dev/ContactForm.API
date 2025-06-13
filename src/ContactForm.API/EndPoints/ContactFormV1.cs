using System;
using ContactForm.API.Interfaces;
using ContactForm.API.Models;
using FastEndpoints;
using FastEndpoints.AspVersioning;

namespace ContactForm.API.EndPoints;

public class ContactFormV1(ILogger<ContactFormV1> _logger, [FromKeyedServices("SMTP")] IEmailService _emailService)
    : Endpoint<ContactFormRequest, Result<string>>
{
    public override void Configure()
    {
        _logger.LogInformation(
            "Configuring ContactFormEndpoint. {0} {1} {2} {3}",
            Http.POST,
            "/api/contact",
            "AllowAnonymous",
            "Version(1, 0)"
        );
        Tags("Contact Form API V1");
        Options(o => o.WithVersionSet("Contact Form API").MapToApiVersion(1.0));
        Verbs(Http.POST);
        Routes("/api/contact");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ContactFormRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Handling request for ContactFormEndpoint.");
        try
        {
            _logger.LogInformation("Sending response...");
            bool IsSuccess = await _emailService.SendEmailAsync(req, ct);
            _logger.LogInformation("Response sent successfully.");
            if (!IsSuccess)
            {
                _logger.LogWarning("Email sending failed.");
                var result = Result<string>.Failure("Failed to send email. Please try again later.");
                await SendAsync(result, cancellation: ct);
                return;
            }
            else
            {
                var result = Result<string>.Success("Email sent successfully");
                _logger.LogInformation("Sending success result to client.");
                await SendAsync(result, cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling the request.");
            var result = Result<string>.Failure("An error occurred while processing your request.");
            await SendAsync(result, cancellation: ct);
        }
    }
}
