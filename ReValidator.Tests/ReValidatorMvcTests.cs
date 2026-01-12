using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ReValidator.SetUp;

namespace ReValidator.Tests;

public class ReValidatorMvcTests
{
    [Fact]
    public async Task Skips_validation_without_attribute()
    {
        using var app = await CreateApplication();

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons/novalidation", new Person());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Validates_when_Validate_attribute_is_used()
    {
        using var app = await CreateApplication();

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons/validate", new Person());

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Valid_object_passes_validation()
    {
        using var app = await CreateApplication();

        var response = await app.GetTestClient()
            .PostAsJsonAsync("/persons/validate", new Person { Name = "John" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ---------------------------------------------------------

    private static async Task<WebApplication> CreateApplication()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddReValidator();
        builder.Services.AddReValidator();

        builder.Services.AddControllers(options => options.Filters.Add<ReValidatorActionFilter>())
            .PartManager.ApplicationParts.Add(
                new Microsoft.AspNetCore.Mvc.ApplicationParts.AssemblyPart(typeof(PersonsController).Assembly));

        var app = builder.Build();

        app.Services.ApplyReconfiguration(new DynamicReconfiguration
        {
            RuleName = "FullNameRequired",
            PropertyName = "Name",
            ErrorMessage = "The \"{PropertyName}\" is required",
            Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
            FullPathToModel = typeof(Person).FullName
        });

        app.MapControllers();

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
