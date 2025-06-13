using System;
using ContactForm.API.Models;

namespace ContactForm.API.Interfaces;

public interface ITemplateService
{
    Task<string> GetTemplateAsync(string templateName, CancellationToken ct = default);
    string ReplacePlaceholders(string template, ContactFormRequest req);
}
