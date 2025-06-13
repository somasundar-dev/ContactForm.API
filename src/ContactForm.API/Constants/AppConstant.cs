namespace ContactForm.API.Constants;

public static class AppConstant
{
    public const string Subject = "Thank you for reaching out!";

    public static class SubmitterPlaceholder
    {
        public const string Name = "#SUBMITTER_NAME#";
        public const string Email = "#SUBMITTER_EMAIL#";
        public const string Message = "#SUBMITTER_MESSAGE#";
    }

    public static class ProfilePlaceholder
    {
        public const string Name = "#NAME#";
        public const string Email = "#EMAIL#";
        public const string Contact = "#CONTACT#";
        public const string Website = "#WEBSITE#";
        public const string Github = "#GITHUB#";
        public const string LinkedIn = "#LINKEDIN#";
        public const string Whatsapp = "#WHATSAPP#";
        public const string Address = "#ADDRESS#";
    }

    public static class Template
    {
        public const string ContactFormResponseHtml = "ContactFormResponse.html";
        public const string ContactFormResponsePlainText = "ContactFormResponse.txt";
    }

    public const string CorsPolicyName = "PrivateCorsPolicy";
}
