using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazor.BrowserExtension.Analyzer
{
    internal static class ExpressionTranslator
    {
        public static string TranslateExpressions(SyntaxNode expression, TranslateContext context)
        {
            // In general, 'Value' in the examples can be a Constant value or a value returned from a method invocation
            if (TryTranslateExpressionStatement(expression, context, out var translated) ||
                TryTranslateLocalDeclarationStatement(expression, context, out translated) ||
                TryTranslateVariableAssignment(expression, context, out translated) ||
                TryTranslateAssignmentExpression(expression, context, out translated) ||
                TryTranslateInvocationExpression(expression, context, out translated) ||
                TryTranslateMemberAccessExpression(expression, context, out translated) ||
                TryTranslateArgument(expression, context, out translated) ||
                TryTranslateIdentifierName(expression, context, out translated) ||
                TryTranslateLiteralExpression(expression, out translated))
            {
                return translated;
            }

            throw new InvalidOperationException($"Unmapped expression type {expression.GetType().Name}. Please raise an issue in GitHub with your code in Main.");
        }

        /// <summary>
        /// Can be any statement.
        /// </summary>
        /// <example>
        /// SomeMethod(args);
        /// </example>
        private static bool TryTranslateExpressionStatement(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is ExpressionStatementSyntax expressionStatement)
            {
                translated = $"{TranslateExpressions(expressionStatement.Expression, context)};";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Variables declared locally.
        /// </summary>
        /// <example>
        /// var variable1 = Value1, variable2 = Value2;
        /// </example>
        private static bool TryTranslateLocalDeclarationStatement(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is LocalDeclarationStatementSyntax localDeclarationStatement)
            {
                var declarations = localDeclarationStatement.Declaration.Variables.Select(variable => TranslateExpressions(variable, context));
                translated = $"let {string.Join(", ", declarations)};";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Variables assigned to a value.
        /// </summary>
        /// <example>
        /// variable1 = Value1
        /// </example>
        private static bool TryTranslateVariableAssignment(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is VariableDeclaratorSyntax variableDeclarator && variableDeclarator.Initializer is EqualsValueClauseSyntax initializeEquals)
            {
                var equalsValue = TranslateExpressions(initializeEquals.Value, context);
                translated = $"{variableDeclarator.Identifier.Text} = {equalsValue}";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Assignment using the equal operator.
        /// </summary>
        /// <example>
        /// obj.Property1 = Value1
        /// or
        /// obj[index1] = Value1
        /// </example>
        private static bool TryTranslateAssignmentExpression(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is AssignmentExpressionSyntax assignmentExpression)
            {
                var assignValue = TranslateExpressions(assignmentExpression.Right, context);
                var assignTo = TranslateExpressions(assignmentExpression.Left, context);

                if (assignTo == "_")
                {
                    // Ignore discard assignments
                    translated = assignValue;
                    return true;
                }

                translated = $"{assignTo} = {assignValue}";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Invocation of a method.
        /// </summary>
        /// <example>
        /// obj.Method1(arg1, arg2)
        /// </example>
        private static bool TryTranslateInvocationExpression(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is InvocationExpressionSyntax invocationExpression)
            {
                var args = invocationExpression.ArgumentList.Arguments.Select(arg => TranslateExpressions(arg, context));
                var invoked = TranslateExpressions(invocationExpression.Expression, context);
                translated = $"{invoked}({string.Join(", ", args)})";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Access a member of a class/object/namespace.
        /// </summary>
        /// <example>
        /// obj.Property1
        /// or
        /// obj.Method1
        /// </example>
        private static bool TryTranslateMemberAccessExpression(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is MemberAccessExpressionSyntax memberAccessExpression)
            {
                var target = TranslateExpressions(memberAccessExpression.Expression, context);
                var jsAccessPath = context.GetJsAccessPath(expression, context.GetSymbol(memberAccessExpression));
                if (jsAccessPath is not null && !context.IsJsAccessPath(memberAccessExpression.Expression))
                {
                    translated = jsAccessPath;
                    return true;
                }

                var member = jsAccessPath ?? memberAccessExpression.Name.ToString();
                translated = $"{target}.{member}";
                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// Argument of a method invocation.
        /// </summary>
        private static bool TryTranslateArgument(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is ArgumentSyntax argument)
            {
                // argument of a method invocation
                var type = context.GetType(argument.Expression);
                translated = TranslateExpressions(argument.Expression, context);
                if (!context.IsJsAccessPath(argument.Expression) && argument.Expression is not LiteralExpressionSyntax)
                {
                    // Pass to JS during initialization
                    var referenceKey = context.AddReference(translated, $"({type.ConvertedType}){translated}");
                    translated = $"fromReference('{referenceKey}')";
                }

                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// A named identifier.
        /// </summary>
        private static bool TryTranslateIdentifierName(SyntaxNode expression, TranslateContext context, out string translated)
        {
            if (expression is IdentifierNameSyntax identifierName)
            {
                var symbol = context.GetSymbol(identifierName);
                translated = identifierName.Identifier.Text;

                var jsAccessPath = context.GetJsAccessPath(expression, symbol);
                if (!string.IsNullOrEmpty(jsAccessPath))
                {
                    translated = jsAccessPath;
                }

                return true;
            }

            translated = null;
            return false;
        }

        /// <summary>
        /// A literal expression.
        /// </summary>
        /// <example>
        /// "a string value"
        /// or
        /// 123.45
        /// </example>
        private static bool TryTranslateLiteralExpression(SyntaxNode expression , out string translated)
        {
            if (expression is LiteralExpressionSyntax literalExpressionSyntax)
            {
                translated = literalExpressionSyntax.Token.Text;
                return true;
            }

            translated = null;
            return false;
        }
    }
}
