﻿using System.Threading.Tasks;
using Xunit;
using RockLibVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    RockLib.Logging.Microsoft.Extensions.Analyzers.LoggerShouldBeSynchronousAnalyzer,
    RockLib.Logging.Microsoft.Extensions.Analyzers.LoggerShouldBeSynchronousCodeFixProvider>;

namespace RockLib.Logging.Microsoft.Extensions.Analyzers.Test
{
    public sealed class LoggerShouldBeSynchronousCodeFixProviderTests
    {
        [Fact]
        public async Task VerifyWhenProcessingModeIsNotSpecified()
        {
            await RockLibVerifier.VerifyCodeFixAsync(@"
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging;
using RockLib.Logging.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.[|AddLogger()|]
            .AddConsoleLogProvider();

        services.AddRockLibLoggerProvider();
    }
}", @"
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging;
using RockLib.Logging.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogger(processingMode: Logger.ProcessingMode.Synchronous)
            .AddConsoleLogProvider();

        services.AddRockLibLoggerProvider();
    }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyWhenProcessingModeIsNotSynchronous()
        {
            await RockLibVerifier.VerifyCodeFixAsync(@"
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging;
using RockLib.Logging.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.[|AddLogger(processingMode: Logger.ProcessingMode.Background)|]
            .AddConsoleLogProvider();

        services.AddRockLibLoggerProvider();
    }
}", @"
using Microsoft.Extensions.DependencyInjection;
using RockLib.Logging;
using RockLib.Logging.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogger(processingMode: Logger.ProcessingMode.Synchronous)
            .AddConsoleLogProvider();

        services.AddRockLibLoggerProvider();
    }
}").ConfigureAwait(false);
        }
    }
}