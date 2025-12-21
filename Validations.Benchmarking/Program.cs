using BenchmarkDotNet.Running;
using Validations.Benchmarking;

BenchmarkRunner.Run<SimpleValidations>();
BenchmarkRunner.Run<NestedValidationsBenchmark>();
BenchmarkRunner.Run<DeepNestedValidationsBenchmark>();