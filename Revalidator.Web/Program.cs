
using ReValidator;
using ReValidator.SetUp;
using static Revalidator.Web.Controllers.WeatherForecastController;

namespace Revalidator.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddReValidator();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Services.ApplyReconfiguration(
                new DynamicReconfiguration
                {
                    RuleName = "FullNameRequired",
                    PropertyName = "Name",
                    ErrorMessage = "The \"{PropertyName}\" is required",
                    Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
                    FullPathToModel = typeof(Person).FullName
                });

            app.Run();
        }
    }
}
