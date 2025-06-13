using System;
using System.Text.Json;
using ContactForm.API.Constants;
using ContactForm.API.Interfaces;
using ContactForm.API.Models;
using Microsoft.Extensions.Options;

namespace ContactForm.API.Services;

public class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;
    private readonly ProfileOptions _profile;

    public TemplateService(ILogger<TemplateService> logger, IOptions<ProfileOptions> profile)
    {
        _logger = logger;
        _profile = profile.Value;
        _logger.LogInformation("TemplateService initialized with profile: {0}", JsonSerializer.Serialize(_profile));
    }

    public async Task<string> GetTemplateAsync(string templateName, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateName);
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }
        ct.ThrowIfCancellationRequested();
        _logger.LogInformation("Reading template file: {0}", templatePath);
        using var reader = new StreamReader(templatePath);
        var content = await reader.ReadToEndAsync(ct);
        _logger.LogInformation("Template file read successfully.");
        return content;
    }

    public string ReplacePlaceholders(string template, ContactFormRequest req)
    {
        _logger.LogInformation("Replacing placeholders in template.");
        _logger.LogInformation("Template: {0}", template);
        _logger.LogInformation("Request: {0}", JsonSerializer.Serialize(req));
        _logger.LogInformation("Profile: {0}", JsonSerializer.Serialize(_profile));

        return template
            .Replace(AppConstant.SubmitterPlaceholder.Name, req.Name)
            .Replace(AppConstant.SubmitterPlaceholder.Email, req.Email)
            .Replace(AppConstant.SubmitterPlaceholder.Message, req.Message)
            .Replace(AppConstant.ProfilePlaceholder.Name, _profile.Name)
            .Replace(AppConstant.ProfilePlaceholder.Email, _profile.Email)
            .Replace(AppConstant.ProfilePlaceholder.Contact, _profile.Contact)
            .Replace(AppConstant.ProfilePlaceholder.Website, _profile.Website)
            .Replace(AppConstant.ProfilePlaceholder.Github, _profile.Github)
            .Replace(AppConstant.ProfilePlaceholder.LinkedIn, _profile.LinkedIn)
            .Replace(AppConstant.ProfilePlaceholder.Whatsapp, _profile.Whatsapp)
            .Replace(AppConstant.ProfilePlaceholder.Address, _profile.Address);
    }
}
