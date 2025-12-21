using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using ReValidator.SetUp;
using FluentValidation;
using Validations.Benchmarking.Models;
using Validations.Benchmarking.Validators;
using ReValidator;

namespace Validations.Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 10, warmupCount: 5)]
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 50, warmupCount: 5)]
    [MemoryDiagnoser]
    [RankColumn]
    public class NestedValidationsBenchmark
    {
        private FluentValidation.IValidator<UserModelNested> _fluentValidator = null!;
        private ReValidator.IValidator<UserModelNested> _reValidator = null!;
        private UserModelNested _model = null!;

        private const int AddressCount = 5;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();

            // FluentValidation
            services.AddSingleton<FluentValidation.IValidator<AddressModel>, AddressModelValidator>();
            services.AddSingleton<FluentValidation.IValidator<UserModelNested>, UserModelNestedValidator>();

            // ReValidator
            services.AddReValidator();

            var provider = services.BuildServiceProvider();

            // ReValidator dynamic rules — User
            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "FirstName",
                ErrorMessage = "The \"{PropertyName}\" is required",
                Expression = "x => !string.IsNullOrWhiteSpace(x.FirstName)",
                FullPathToModel = typeof(UserModelNested).FullName
            });

            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "LastName",
                ErrorMessage = "The \"{PropertyName}\" is required",
                Expression = "x => !string.IsNullOrWhiteSpace(x.LastName)",
                FullPathToModel = typeof(UserModelNested).FullName
            });

            // ReValidator dynamic rules — Address
            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "Street",
                ErrorMessage = "The \"{PropertyName}\" is required",
                Expression = "x => x.Addresses.All(a => !string.IsNullOrWhiteSpace(a.Street))",
                FullPathToModel = typeof(UserModelNested).FullName
            });

            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "City",
                ErrorMessage = "The \"{PropertyName}\" is required",
                Expression = "x => x.Addresses.All(a => !string.IsNullOrWhiteSpace(a.City))",
                FullPathToModel = typeof(UserModelNested).FullName
            });

            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "Country",
                ErrorMessage = "The \"{PropertyName}\" is required",
                Expression = "x => x.Addresses.All(a => !string.IsNullOrWhiteSpace(a.Country))",
                FullPathToModel = typeof(UserModelNested).FullName
            });

            _fluentValidator = provider.GetRequiredService<FluentValidation.IValidator<UserModelNested>>();
            _reValidator = provider.GetRequiredService<ReValidator.IValidator<UserModelNested>>();

            _model = new UserModelNested
            {
                FirstName = "John",
                LastName = "Doe",
                Addresses = Enumerable.Range(0, AddressCount)
                    .Select(_ => new AddressModel
                    {
                        Street = "Main",
                        City = "NY",
                        Country = "US"
                    })
                    .ToArray()
            };

            // 🔥 warm-up
            _fluentValidator.Validate(_model);
            _reValidator.Validate(_model);
        }

        [Benchmark]
        public bool ValidateNestedWithReValidator()
            => _reValidator.Validate(_model).IsValid;

        [Benchmark]
        public bool ValidateNestedWithFluentValidation()
            => _fluentValidator.Validate(_model).IsValid;
    }
}
