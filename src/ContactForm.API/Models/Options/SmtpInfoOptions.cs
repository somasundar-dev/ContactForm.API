namespace ContactForm.API.Models;

public class SmtpInfoOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool UseSSL { get; set; } = true;
}
