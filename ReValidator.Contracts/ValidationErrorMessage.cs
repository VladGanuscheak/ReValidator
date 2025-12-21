using System;

namespace ReValidator
{
    public class ValidationErrorMessage
    {
        public string PropertyName { get; set; } = string.Empty;

        public string[] ErrorMessages { get; set; } 
            = Array.Empty<string>();
    }
}
