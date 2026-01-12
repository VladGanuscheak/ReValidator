using Microsoft.AspNetCore.Http;

namespace ReValidator.Validation.Mvc
{
    public sealed class ReValidationProblem
    {
        public string Type { get; init; } = "about:blank";
        public string Title { get; init; } = "Validation failed";
        public int Status { get; init; } = StatusCodes.Status422UnprocessableEntity;
        public string Detail { get; init; } = "One or more validation rules were violated.";

        public IDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
    }
}
