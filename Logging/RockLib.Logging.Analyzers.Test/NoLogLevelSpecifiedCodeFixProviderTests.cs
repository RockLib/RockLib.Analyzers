﻿using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Analyzers.Test
{
    public class NoLogLevelSpecifiedCodeFixProviderTests
    {
        [Fact(DisplayName = null)]
        public async Task CodeFixApplied1()
        {
            await TestAssistants.VerifyCodeFixAsync<NoLogLevelSpecifiedAnalyzer, NoLogLevelSpecifiedCodeFixProvider>(@"
using RockLib.Logging;
using System;

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        logger.Log([|new LogEntry()|]);

        logger.Log([|new LogEntry { Message = ""Hello, world!"" }|]);

        logger.Log([|new LogEntry(""Hello, world!"")|]);

        LogEntry logEntry1 = new();
        logger.Log([|logEntry1|]);

        LogEntry logEntry2 = new(""Hello, world!"");
        logger.Log([|logEntry2|]);
    }
}", @"
using RockLib.Logging;
using System;

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        logger.Log(new LogEntry() { Level = LogLevel.Debug });

        logger.Log(new LogEntry { Message = ""Hello, world!"", Level = LogLevel.Debug });

        logger.Log(new LogEntry(""Hello, world!"", level: LogLevel.Debug));

        LogEntry logEntry1 = new()
        {
            Level = LogLevel.Debug
        };
        logger.Log(logEntry1);

        LogEntry logEntry2 = new(""Hello, world!"", level: LogLevel.Debug);
        logger.Log(logEntry2);
    }
}").ConfigureAwait(false);
        }
    }
}
