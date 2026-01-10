using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using ReValidator.SetUp;
using ReValidator.Validation.MinimalApi;

namespace ReValidator.Tests;

public class ReValidatorEndpointFilterTests
{
    [Fact]
    public async Task Skips_validation_without_filter()
    {
        using var app = await CreateApplication(app =>
        {
            app.MapPost("/persons", (Person p) => Results.Ok());
        });

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons", new Person());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Validates_when_typed_ReValidator_is_used()
    {
        using var app = await CreateApplication(app =>
        {
            app.MapPost("/persons", (Person p) => Results.Ok())
               .AddReValidator<Person>();
        });

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons", new Person());

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Validates_when_generic_ReValidatorFilter_is_used()
    {
        using var app = await CreateApplication(app =>
        {
            app.MapPost("/persons", (Person p) => Results.Ok())
               .AddEndpointFilter<ReValidatorFilter>();
        });

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons", new Person());

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Valid_object_passes_validation()
    {
        using var app = await CreateApplication(app =>
        {
            app.MapPost("/persons", (Person p) => Results.Ok())
               .AddReValidator<Person>();
        });

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons", new Person { Name = "John" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ---------------------------------------------------------

    private static async Task<WebApplication> CreateApplication(Action<WebApplication> configure)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        builder.Services.AddReValidator();

        var app = builder.Build();

        app.Services.ApplyReconfiguration(new DynamicReconfiguration
        {
            RuleName = "FullNameRequired",
            PropertyName = "Name",
            ErrorMessage = "The \"{PropertyName}\" is required",
            Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
            FullPathToModel = typeof(Person).FullName
        });

        configure(app);

        await app.StartAsync();
        return app;
    }

    // ---------------------------------------------------------

    public sealed class Person
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
