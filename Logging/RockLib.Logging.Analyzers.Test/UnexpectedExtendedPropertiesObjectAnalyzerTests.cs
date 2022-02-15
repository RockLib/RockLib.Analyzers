﻿using Xunit;
using System.Threading.Tasks;

namespace RockLib.Logging.Analyzers.Test
{
    public class UnexpectedExtendedPropertiesObjectAnalyzerTests
    {
        [Fact(DisplayName = "Diagnostics are reported when logging with a non-anon type")]
        public async Task DiagnosticReported1()
        {
            await TestAssistants.VerifyAnalyzerAsync<UnexpectedExtendedPropertiesObjectAnalyzer>(@"
using RockLib.Logging;
using RockLib.Logging.SafeLogging;
using System;
using System.Collections.Generic;
public class Derp
{
	public string Value { get; set; }
}

public class TestClass
{
	public void Warn_All(       
        ILogger logger)
	{
		[|logger.DebugSanitized(""Debug Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.WarnSanitized(""Warn Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.InfoSanitized(""Info Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.ErrorSanitized(""Error Message"", new Derp(){ Value = ""florp"" })|];

		[|logger.Debug(""Debug Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.Warn(""Warn Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.Info(""Info Message"", new Derp(){ Value = ""florp"" })|];
		[|logger.Error(""Error Message"", new Derp(){ Value = ""florp"" })|];
	}
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "Diagnostics are not reported when logging with a anon type")]
        public async Task DiagnosticReported2()
        {
            await TestAssistants.VerifyAnalyzerAsync<UnexpectedExtendedPropertiesObjectAnalyzer>(@"
using RockLib.Logging;
using RockLib.Logging.SafeLogging;
using System;
using System.Collections.Generic;

public class TestClass
{
	public void Do_Not_Warn(       
        ILogger logger)
	{
		logger.DebugSanitized(""Debug Message"", new { Value = ""florp""});
		logger.WarnSanitized(""Warn Message"", new { Value = ""florp""});
		logger.InfoSanitized(""Info Message"", new { Value = ""florp""});
		logger.ErrorSanitized(""Error Message"", new { Value = ""florp""});

		logger.Debug(""Debug Message"", new { Value = ""florp""});
		logger.Warn(""Warn Message"", new { Value = ""florp""});
		logger.Info(""Info Message"", new { Value = ""florp""});
		logger.Error(""Error Message"", new { Value = ""florp""});
		
		var dictionary = new Dictionary<string, object>();
		dictionary.Add(""glip"", ""glop"");
		logger.DebugSanitized(""DictionaryDebug Message"", dictionary);
	}
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "Diagnostics are reported when initializing LogEntry with non-anonymous extended prop")]
        public async Task DiagnosticReported3()
        {
            await TestAssistants.VerifyAnalyzerAsync<UnexpectedExtendedPropertiesObjectAnalyzer>(
                @"
using RockLib.Logging;
using RockLib.Logging.SafeLogging;
using System;
using System.Collections.Generic;
public class Derp
{
	public string Value { get; set; }
}

public class TestClass
{
	public void Warn_LogEntry()
	{
		var entry = [|new LogEntry(""message 1"", extendedProperties: new Derp())|];
	}
}").ConfigureAwait(false);
        }

        [Fact(DisplayName = "Diagnostics are not reported when initializing LogEntry with anonymous extended prop")]
        public async Task DiagnosticReported4()
        {
            await TestAssistants.VerifyAnalyzerAsync<UnexpectedExtendedPropertiesObjectAnalyzer>(
                @"
using RockLib.Logging;
using RockLib.Logging.SafeLogging;
using System;
using System.Collections.Generic;

public class TestClass
{
	public void Warn_LogEntry()
	{
		var entry = new LogEntry(""message 1"", extendedProperties: new { Flip = ""Florp"" });
		var entry2 = new LogEntry(""message 1"", extendedProperties: new Dictionary<string, string>());	
	}
}").ConfigureAwait(false);
        }
    }
}
