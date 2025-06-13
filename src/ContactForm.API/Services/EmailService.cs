using System;
using System.Text.Json;
using ContactForm.API.Constants;
using ContactForm.API.Interfaces;
using ContactForm.API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ContactForm.API.Services;

public class SMTPEmailService : IEmailService
{
    private readonly ILogger<SMTPEmailService> _logger;
    private readonly ITemplateService _templateService;
    private readonly SmtpInfoOptions _smtpInfo;
    private readonly ProfileOptions _profile;

    public SMTPEmailService(
        ILogger<SMTPEmailService> logger,
        IOptions<SmtpInfoOptions> smtpInfo,
        IOptions<ProfileOptions> profile,
        ITemplateService templateService
    )
    {
        _smtpInfo = smtpInfo?.Value ?? throw new ArgumentNullException(nameof(smtpInfo));
        _profile = profile?.Value ?? throw new ArgumentNullException(nameof(profile));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("SMTPEmailService initialized.");
    }

    public async Task<bool> SendEmailAsync(ContactFormRequest req, CancellationToken ct = default)
    {
        _logger.LogInformation("Preparing to send email to {0} from {1}", req.Email, _smtpInfo.Username);
        _logger.LogInformation("Request: {0}", JsonSerializer.Serialize(req));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpInfo.DisplayName, _smtpInfo.Username));
        message.To.Add(new MailboxAddress(req.Name, req.Email));
        message.Bcc.Add(new MailboxAddress(_profile.Name, _profile.Email));
        message.Subject = AppConstant.Subject;
        _logger.LogInformation("Email subject set to: {0}", AppConstant.Subject);
        _logger.LogInformation("Email message created.");

        _logger.LogInformation("Loading email templates...");
        var textBodyTemplate = await _templateService.GetTemplateAsync(
            AppConstant.Template.ContactFormResponsePlainText,
            ct
        );
        var htmlBodyTemplate = await _templateService.GetTemplateAsync(
            AppConstant.Template.ContactFormResponseHtml,
            ct
        );
        _logger.LogInformation("Email templates loaded successfully.");
        _logger.LogInformation("Replacing placeholders in templates...");
        var textBody = _templateService.ReplacePlaceholders(textBodyTemplate, req);
        var htmlBody = _templateService.ReplacePlaceholders(htmlBodyTemplate, req);

        var bodyBuilder = new BodyBuilder { TextBody = textBody, HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        _logger.LogInformation("Connecting to SMTP server...");
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpInfo.Host, _smtpInfo.Port, SecureSocketOptions.StartTls);
        _logger.LogInformation("Connected to SMTP server.");

        _logger.LogInformation("Authenticating SMTP client...");
        await client.AuthenticateAsync(_smtpInfo.Username, _smtpInfo.Password);
        _logger.LogInformation("SMTP client authenticated.");

        _logger.LogInformation("Sending email...");
        await client.SendAsync(message);
        _logger.LogInformation("Email sent successfully.");

        client.Disconnect(true);
        _logger.LogInformation("Disconnected from SMTP server.");
        return true;
    }
}
