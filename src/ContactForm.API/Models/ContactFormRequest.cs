using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContactForm.API.Models;

public class ContactFormRequest
{
    [Required]
    [JsonPropertyName("name")]
    [Display(Name = "Full Name")]
    [DataType(DataType.Text)]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    [DataType(DataType.EmailAddress)]
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 10)]
    [DataType(DataType.MultilineText)]
    [JsonPropertyName("message")]
    [Display(Name = "Message")]
    public required string Message { get; set; }
}
