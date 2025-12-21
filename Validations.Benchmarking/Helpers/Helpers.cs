namespace Validations.Benchmarking.Helpers
{
    public sealed class Helpers
    {
        public static bool IsValidEmail(string email)
            => System.Text.RegularExpressions.Regex.IsMatch(
                email ?? string.Empty,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
    }

}
