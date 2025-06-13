using System;
using System.Text.Json;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using ContactForm.API.Constants;
using ContactForm.API.Interfaces;
using ContactForm.API.Models;
using Microsoft.Extensions.Options;

namespace ContactForm.API.Services;

public class SESService : IEmailService
{
    private readonly ILogger<SESService> _logger;
    private readonly ITemplateService _templateService;
    private readonly ProfileOptions _profile;
    private readonly IAmazonSimpleEmailServiceV2 _amazonSimpleEmailService;

    public SESService(
        ILogger<SESService> logger,
        ITemplateService templateService,
        IAmazonSimpleEmailServiceV2 amazonSimpleEmailService,
        IOptions<ProfileOptions> profileOptions
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _amazonSimpleEmailService =
            amazonSimpleEmailService ?? throw new ArgumentNullException(nameof(amazonSimpleEmailService));
        _profile = profileOptions?.Value ?? throw new ArgumentNullException(nameof(profileOptions));
        _logger.LogInformation("SESService initialized.");
    }

    public async Task<bool> SendEmailAsync(ContactFormRequest req, CancellationToken ct = default)
    {
        _logger.LogInformation("Preparing to send email to {0} from AWS SES", req.Email);
        _logger.LogInformation("Request: {0}", JsonSerializer.Serialize(req));
        ct.ThrowIfCancellationRequested();

        _logger.LogInformation("Loading email templates...");
        var mailBody = new Body();

        _logger.LogInformation("Loading HTML template...");
        var htmlBodyTemplate = await _templateService.GetTemplateAsync(
            AppConstant.Template.ContactFormResponseHtml,
            ct
        );

        _logger.LogInformation("Loading plain text template...");
        var plainTextTemplate = await _templateService.GetTemplateAsync(
            AppConstant.Template.ContactFormResponsePlainText,
            ct
        );

        _logger.LogInformation("Replacing placeholders in templates...");

        _logger.LogInformation("Replacing placeholders in HTML template...");
        var htmlBody = _templateService.ReplacePlaceholders(htmlBodyTemplate, req);

        _logger.LogInformation("Replacing placeholders in plain text template...");
        var plainTextBody = _templateService.ReplacePlaceholders(plainTextTemplate, req);

        _logger.LogInformation("Setting email body content...");
        mailBody.Html = new Content { Data = htmlBody };
        mailBody.Text = new Content { Data = plainTextBody };

        _logger.LogInformation("Email templates loaded successfully.");
        var message = new Message
        {
            Body = mailBody,
            Subject = new Content { Data = AppConstant.Subject },
        };
        _logger.LogInformation("Email subject set to: {0}", AppConstant.Subject);
        _logger.LogInformation("Email message created.");

        _logger.LogInformation("Creating email destination...");
        var destination = new Destination { ToAddresses = [req.Email], BccAddresses = [_profile.Email] };
        _logger.LogInformation("Email destination created for {0} and BCC to {1}", req.Email, _profile.Email);

        _logger.LogInformation("Creating SendEmailRequest...");
        var request = new SendEmailRequest
        {
            FromEmailAddress = _profile.Email,
            Destination = destination,
            Content = new EmailContent { Simple = message },
        };

        _logger.LogInformation("Sending email...");
        var response = await _amazonSimpleEmailService.SendEmailAsync(request, ct);
        _logger.LogInformation("Response received from AWS SES.");

        _logger.LogInformation("Email Response: {0}", JsonSerializer.Serialize(response));
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            _logger.LogError("Failed to send email. Status Code: {0}", response.HttpStatusCode);
            return false;
        }
        _logger.LogInformation("Email sent to {0} with Message ID: {1}", req.Email, response.MessageId);
        return true;
    }
}
