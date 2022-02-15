using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Analyzers.Test
{
    public class UseSanitizingLoggingMethodCodeFixProviderTests
    {
        [Fact(DisplayName = "'Change to SetSanitizedExtendedProperties' is applied")]
        public async Task CodeFixApplied1()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        [|logEntry.SetExtendedProperties(new { foo = this })|];
    }
}", @"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        logEntry.SetSanitizedExtendedProperties(new { foo = this });
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "'Change to sanitizing logging extension method' is applied")]
        public async Task CodeFixApplied2()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz(ILogger logger)
    {
        [|logger.Warn(""Hello, world!"", new { foo = this })|];
    }
}", @"
using RockLib.Logging;
using RockLib.Logging.SafeLogging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz(ILogger logger)
    {
        logger.WarnSanitized(""Hello, world!"", new { foo = this });
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "'Replace extendedProperties parameter with call to SetSanitizedExtendedProperties method' is applied")]
        public async Task CodeFixApplied3()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;
using System;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = [|new LogEntry(""Hello, world!"", extendedProperties: new { foo = this })
        {
            CorrelationId = Guid.NewGuid().ToString()
        }|];
    }
}", @"
using RockLib.Logging;
using System;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry(""Hello, world!"")
        {
            CorrelationId = Guid.NewGuid().ToString()
        }.SetSanitizedExtendedProperties(new { foo = this });
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "'Replace with call to SetSanitizedExtendedProperty' is applied to indexer")]
        public async Task CodeFixApplied4()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        [|logEntry.ExtendedProperties[""bar""] = this|];
    }
}", @"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        logEntry.SetSanitizedExtendedProperty(""bar"", this);
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "'Replace with call to SetSanitizedExtendedProperty' is applied to Add method")]
        public async Task CodeFixApplied5()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        [|logEntry.ExtendedProperties.Add(""bar"", this)|];
    }
}", @"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        logEntry.SetSanitizedExtendedProperty(""bar"", this);
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "'Replace with call to SetSanitizedExtendedProperty' is applied to TryAdd method")]
        public async Task CodeFixApplied6()
        {
            await TestAssistants.VerifyCodeFixAsync<UseSanitizingLoggingMethodAnalyzer, UseSanitizingLoggingMethodCodeFixProvider>(@"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        [|logEntry.ExtendedProperties.TryAdd(""bar"", this)|];
    }
}", @"
using RockLib.Logging;

public class Foo
{
    public string Bar { get; set; }

    public void Baz()
    {
        var logEntry = new LogEntry();
        logEntry.SetSanitizedExtendedProperty(""bar"", this);
    }
}").ConfigureAwait(false);
        }
    }
}
