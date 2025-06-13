using System;
using ContactForm.API.Models;

namespace ContactForm.API.Interfaces;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(ContactFormRequest req, CancellationToken ct = default);
}
