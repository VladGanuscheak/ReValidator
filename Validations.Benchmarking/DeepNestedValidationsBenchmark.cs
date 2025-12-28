using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using ReValidator.SetUp;
using Validations.Benchmarking.Models;
using Validations.Benchmarking.Validators;
using ReValidator;
using RV = ReValidator;
using FV = FluentValidation;

namespace Validations.Benchmarking
{
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 10, warmupCount: 5)]
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 50, warmupCount: 5)]
    [MemoryDiagnoser]
    [RankColumn]
    public class DeepNestedValidationsBenchmark
    {
        private FV.IValidator<UserModelDeep> _fluentValidator = null!;
        private RV.IValidator<UserModelDeep> _reValidator = null!;
        private UserModelDeep _model = null!;

        private const int OrderCount = 3;
        private const int ItemsPerOrder = 4;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();

            // FluentValidation
            services.AddSingleton<FV.IValidator<ProductModel>, ProductModelValidator>();
            services.AddSingleton<FV.IValidator<OrderItemModel>, OrderItemModelValidator>();
            services.AddSingleton<FV.IValidator<OrderModel>, OrderModelValidator>();
            services.AddSingleton<FV.IValidator<UserModelDeep>, UserModelDeepValidator>();

            // ReValidator
            services.AddReValidator(options =>
            {
                options.RegisterType<Validations.Benchmarking.Helpers.Helpers>();
            });

            var provider = services.BuildServiceProvider();

            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "CustomEmailValidation",
                PropertyName = "Email",
                ErrorMessage = "User.Email.Invalid",
                Expression = "x => Validations.Benchmarking.Helpers.Helpers.IsValidEmail(x.Email)",
                FullPathToModel = typeof(UserModelDeep).FullName,
                
            });

            // Dynamic rules for UserModelDeep
            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "FirstName",
                ErrorMessage = "FirstName required",
                Expression = "x => !string.IsNullOrWhiteSpace(x.FirstName)",
                FullPathToModel = typeof(UserModelDeep).FullName
            });

            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "LastName",
                ErrorMessage = "LastName required",
                Expression = "x => !string.IsNullOrWhiteSpace(x.LastName)",
                FullPathToModel = typeof(UserModelDeep).FullName
            });

            // Orders
            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "OrderNumber",
                ErrorMessage = "OrderNumber required",
                Expression = "x => x.Orders.All(o => !string.IsNullOrWhiteSpace(o.OrderNumber))",
                FullPathToModel = typeof(UserModelDeep).FullName
            });

            // Items in Orders
            provider.ApplyReconfiguration(new DynamicReconfiguration
            {
                RuleName = "RequiredProperty",
                PropertyName = "ProductName",
                ErrorMessage = "Product Name required",
                Expression = "x => x.Orders.All(o => o.Items.All(i => !string.IsNullOrWhiteSpace(i.Product.Name)))",
                FullPathToModel = typeof(UserModelDeep).FullName
            });


            _fluentValidator = provider.GetRequiredService<FV.IValidator<UserModelDeep>>();
            _reValidator = provider.GetRequiredService<ReValidator.IValidator<UserModelDeep>>();

            // Sample data
            _model = new UserModelDeep
            {
                Email = "uasea@rtest.com",
                FirstName = "John",
                LastName = "Doe",
                Orders = [.. Enumerable.Range(0, OrderCount)
                    .Select(_ => new OrderModel
                    {
                        OrderNumber = "ORD-001",
                        Items = [.. Enumerable.Range(0, ItemsPerOrder)
                            .Select(__ => new OrderItemModel
                            {
                                Quantity = 1,
                                Product = new ProductModel { Name = "Product", Price = 10.0m }
                            })]
                    })]
            };

            // 🔥 warm-up
            _fluentValidator.Validate(_model);
            _reValidator.Validate(_model);
        }

        [Benchmark]
        public bool ValidateDeepWithReValidator() => _reValidator.Validate(_model).IsValid;

        [Benchmark]
        public bool ValidateDeepWithFluentValidation() => _fluentValidator.Validate(_model).IsValid;
    }
}
