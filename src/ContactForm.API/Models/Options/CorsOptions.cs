using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactForm.API.Models
{
    public class CorsOptions
    {
        public string Origins { get; set; } = string.Empty;
        public string Methods { get; set; } = string.Empty;
        public string Headers { get; set; } = string.Empty;
        public string ExposedHeaders { get; set; } = string.Empty;
        public bool AllowCredentials { get; set; } = false;
    }
}
