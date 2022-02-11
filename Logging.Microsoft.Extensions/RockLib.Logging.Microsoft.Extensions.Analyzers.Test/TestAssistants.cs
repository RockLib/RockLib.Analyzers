using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace RockLib.Logging.Microsoft.Extensions.Analyzers.Test
{
    internal static class TestAssistants
    {
        public static async Task VerifyAnalyzerAsync<T>(string source)
            where T : DiagnosticAnalyzer, new()
        {
            var test = new CSharpCodeFixTest<T, EmptyCodeFixProvider, XUnitVerifier>
            {
                TestCode = source,
                ReferenceAssemblies = ReferenceAssemblies.Default
                    .AddPackages(ImmutableArray.Create(
                        new PackageIdentity("RockLib.Logging.Microsoft.Extensions", "1.0.2"),
                        new PackageIdentity("Microsoft.Extensions.Hosting", "5.0.0")))
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}