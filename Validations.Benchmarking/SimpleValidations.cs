using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using ReValidator.SetUp;
using Validations.Benchmarking.Models;
using ReValidator;
using FluentValidation;

namespace Validations.Benchmarking
{
    // Compare short vs long iteration profiles
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 10, warmupCount: 5)]
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 50, warmupCount: 5)]
    [MemoryDiagnoser]
    [RankColumn]
    public class SimpleValidations
    {
        private FluentValidation.IValidator<UserModel> _fluentValidator = null!;
        private ReValidator.IValidator<UserModel> _reValidator = null!;
        private UserModel _userModel = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<UserModel>();

            // ReValidator
            services.AddReValidator();

            var serviceProvider = services.BuildServiceProvider();

            // Dynamic rules configuration
            serviceProvider.ApplyReconfiguration(
                new DynamicReconfiguration
                {
                    RuleName = "RequiredProperty",
                    PropertyName = "FirstName",
                    ErrorMessage = "The \"{PropertyName}\" is required",
                    Expression = "x => !string.IsNullOrWhiteSpace(x.FirstName)",
                    FullPathToModel = typeof(UserModel).FullName
                });

            serviceProvider.ApplyReconfiguration(
                new DynamicReconfiguration
                {
                    RuleName = "RequiredProperty",
                    PropertyName = "LastName",
                    ErrorMessage = "The \"{PropertyName}\" is required",
                    Expression = "x => !string.IsNullOrWhiteSpace(x.LastName)",
                    FullPathToModel = typeof(UserModel).FullName
                });

            _fluentValidator = serviceProvider
                .GetRequiredService<FluentValidation.IValidator<UserModel>>();

            _reValidator = serviceProvider
                .GetRequiredService<ReValidator.IValidator<UserModel>>();

            _userModel = new UserModel
            {
                FirstName = "",
                LastName = "Doe"
            };

            // 🔥 Warm compiled expressions & caches
            _fluentValidator.Validate(_userModel);
            _reValidator.Validate(_userModel);
        }

        [Benchmark]
        public bool ValidateWithReValidator()
            => _reValidator.Validate(_userModel).IsValid;

        [Benchmark]
        public bool ValidateWithFluentValidator()
            => _fluentValidator.Validate(_userModel).IsValid;
    }
}
