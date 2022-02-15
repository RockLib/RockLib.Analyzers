using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.Analyzers.Test
{
    public class UnexpectedExtendedPropertiesCodeFixTests
    {
        [Fact(DisplayName = "Code fix applied when extended properties are not provided as an anonymous object")]
        public async Task CodeFixApplied1()
        {
            await TestAssistants.VerifyCodeFixAsync<UnexpectedExtendedPropertiesObjectAnalyzer, UnexpectedExtendedPropertiesCodeFixProvider>(@"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        var anonymousFlorp = new Florp(""alakazam"");
        [|logger.Info(""no good message"", anonymousFlorp)|];
    }
}", @"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        var anonymousFlorp = new Florp(""alakazam"");
        logger.Info(""no good message"", new { anonymousFlorp });
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "Code fix applied when extended properties are not provided as an anonymous object with object initializer")]
        public async Task CodeFixApplied2()
        {
            await TestAssistants.VerifyCodeFixAsync<UnexpectedExtendedPropertiesObjectAnalyzer, UnexpectedExtendedPropertiesCodeFixProvider>(@"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        [|logger.Info(""no good message"", new Florp(""greninja""))|];
    }
}", @"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        logger.Info(""no good message"", new { Florp = new Florp(""greninja"") });
    }
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "Code fix applied when extended properties are not provided as an anonymous in Logging method")]
        public async Task CodeFixApplied3()
        {
            await TestAssistants.VerifyCodeFixAsync<UnexpectedExtendedPropertiesObjectAnalyzer, UnexpectedExtendedPropertiesCodeFixProvider>(@"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        var logEntry = [|new LogEntry(""no good message"", extendedProperties: new Florp(""frogadier""))|];
        logger.Log(logEntry);
    }
}", @"
using RockLib.Logging;
using System;
public class Florp
{
    public Florp(string grelp)
    {
        Grelp = grelp;
    }
    public string Grelp { get; }
}

public class Test
{
    public void Call_Log_With_LogEntry_With_Level_Not_Set(ILogger logger)
    {
        var logEntry = new LogEntry(""no good message"", extendedProperties: new { Florp = new Florp(""frogadier"") });
        logger.Log(logEntry);
    }
}").ConfigureAwait(false);
        }
    }
}
