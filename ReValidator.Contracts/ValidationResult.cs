using System;

namespace ReValidator
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Length == 0;

        public ValidationErrorMessage[] Errors { get; set; } 
            = Array.Empty<ValidationErrorMessage>();
    }
}
