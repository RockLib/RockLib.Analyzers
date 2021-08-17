﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;
using RockLib.Analyzers.Common;
using System.Collections.Immutable;

namespace RockLib.Logging.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CaughtExceptionShouldBeLoggedAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString _title = "Caught exception should be logged";
        private static readonly LocalizableString _messageFormat = "The caught exception should be passed into the logging method";
        private static readonly LocalizableString _description = "If a logging method is inside a catch block, the caught exception should be passed to it.";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticIds.CaughtExceptionShouldBeLogged,
            _title,
            _messageFormat,
            DiagnosticCategory.Usage,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: _description,
            helpLinkUri: string.Format(HelpLinkUri.Format, DiagnosticIds.CaughtExceptionShouldBeLogged));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            var loggingExtensionsType = context.Compilation.GetTypeByMetadataName("RockLib.Logging.LoggingExtensions");
            if (loggingExtensionsType == null)
                return;

            var safeLoggingExtensionsType = context.Compilation.GetTypeByMetadataName("RockLib.Logging.SafeLogging.SafeLoggingExtensions");
            if (safeLoggingExtensionsType == null)
                return;

            var loggerType = context.Compilation.GetTypeByMetadataName("RockLib.Logging.ILogger");
            if (loggerType == null)
                return;

            var exceptionType = context.Compilation.GetTypeByMetadataName("System.Exception");
            if (exceptionType == null)
                return;

            var analyzer = new InvocationOperationAnalyzer(loggingExtensionsType, exceptionType, safeLoggingExtensionsType, loggerType);
            context.RegisterOperationAction(analyzer.Analyze, OperationKind.Invocation);
        }

        private class InvocationOperationAnalyzer
        {
            private readonly INamedTypeSymbol _loggingExtensionsType;
            private readonly INamedTypeSymbol _safeLoggingExtensionsType;
            private readonly INamedTypeSymbol _loggerType;
            private readonly INamedTypeSymbol _exceptionType;

            public InvocationOperationAnalyzer(INamedTypeSymbol loggingExtensionsType,
                INamedTypeSymbol exceptionType,
                INamedTypeSymbol safeLoggingExtensionsType,
                INamedTypeSymbol loggerType)
            {
                _loggingExtensionsType = loggingExtensionsType;
                _exceptionType = exceptionType;
                _safeLoggingExtensionsType = safeLoggingExtensionsType;
                _loggerType = loggerType;
            }

            public void Analyze(OperationAnalysisContext context)
            {
                var invocationOperation = (IInvocationOperation)context.Operation;
                var methodSymbol = invocationOperation.TargetMethod;

                if (methodSymbol.MethodKind != MethodKind.Ordinary
                    || !(GetCatchClause(invocationOperation) is ICatchClauseOperation catchClause))
                {
                    return;
                }

                if (SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, _loggingExtensionsType)
                    || SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, _safeLoggingExtensionsType))
                {
                    var visitor = new CatchParameterWalker(invocationOperation, _exceptionType, context.Compilation);
                    visitor.Visit(catchClause);
                    if (visitor.IsExceptionCaught)
                        return;
                }
                else if (methodSymbol.Name == "Log"
                    && SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, _loggerType))
                {
                    var logEntryArgument = invocationOperation.Arguments[0];
                    var logEntryCreation = logEntryArgument.GetLogEntryCreationOperation();

                    if (logEntryCreation == null
                        || IsExceptionSet(logEntryCreation, logEntryArgument.Value, catchClause, context.Compilation))
                    {
                        return;
                    }
                }
                else
                    return;

                var diagnostic = Diagnostic.Create(Rule, invocationOperation.Syntax.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            private ICatchClauseOperation GetCatchClause(IInvocationOperation invocationOperation)
            {
                var parent = invocationOperation.Parent;
                while (parent != null)
                {
                    //TODO: Other catch operations?
                    if (parent is ICatchClauseOperation catchClause)
                        return catchClause;
                    parent = parent.Parent;
                }
                return null;
            }

            private bool IsExceptionSet(IObjectCreationOperation logEntryCreation, IOperation logEntryArgumentValue,
                ICatchClauseOperation catchClause, Compilation compilation)
            {
                if (catchClause.ExceptionDeclarationOrExpression is null)
                    return false;

                var logWalker = new LogEntryCreatedWalker(logEntryArgumentValue, logEntryCreation, _exceptionType, compilation);
                logWalker.Visit(logEntryCreation.GetRootOperation());
                return logWalker.IsExceptionSet;
            }
        }
    }
}
