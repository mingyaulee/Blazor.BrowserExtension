using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazor.BrowserExtension.Analyzer.Translation
{
    internal static class ExpressionTranslator
    {
        public static string Translate(SyntaxNode expression, TranslateContext context)
            => expression switch
            {
                // In general, 'Value' in the examples can be a Constant value or a value returned from a method invocation
                ExpressionStatementSyntax expressionStatement => TranslateExpressionStatement(expressionStatement, context),
                LocalDeclarationStatementSyntax localDeclarationStatement => TranslateLocalDeclarationStatement(localDeclarationStatement, context),
                VariableDeclaratorSyntax variableDeclarator when variableDeclarator.Initializer is not null => TranslateVariableAssignment(variableDeclarator, context),
                AssignmentExpressionSyntax assignmentExpression => TranslateAssignmentExpression(assignmentExpression, context),
                ObjectCreationExpressionSyntax objectCreationExpression => TranslateObjectCreationExpression(objectCreationExpression, context),
                AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpression => TranslateAnonymousObjectCreationExpression(anonymousObjectCreationExpression, context),
                ArrayCreationExpressionSyntax arrayCreationExpression => TranslateArrayCreationExpression(arrayCreationExpression, context),
                ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression => TranslateImplicitArrayCreationExpression(implicitArrayCreationExpression, context),
                CollectionExpressionSyntax collectionExpression => TranslateCollectionExpression(collectionExpression, context),
                InvocationExpressionSyntax invocationExpression => TranslateInvocationExpression(invocationExpression, context),
                MemberAccessExpressionSyntax memberAccessExpression => TranslateMemberAccessExpression(memberAccessExpression, context),
                ArgumentSyntax argument => TranslateArgument(argument, context),
                IdentifierNameSyntax identifierName => TranslateIdentifierName(identifierName, context),
                AliasQualifiedNameSyntax aliasQualifiedName => TranslateAliasQualifiedName(aliasQualifiedName, context),
                LiteralExpressionSyntax literalExpression => TranslateLiteralExpression(literalExpression),
                _ => throw new InvalidOperationException($"Unmapped expression type {expression.GetType().Name}. Please raise an issue in GitHub with your code in Main.")
            };

        /// <summary>
        /// Can be any statement.
        /// </summary>
        /// <example>
        /// SomeMethod(args);
        /// </example>
        private static string TranslateExpressionStatement(ExpressionStatementSyntax expressionStatement, TranslateContext context) => $"{Translate(expressionStatement.Expression, context)};";

        /// <summary>
        /// Variables declared locally.
        /// </summary>
        /// <example>
        /// var variable1 = Value1, variable2 = Value2;
        /// </example>
        private static string TranslateLocalDeclarationStatement(LocalDeclarationStatementSyntax localDeclarationStatement, TranslateContext context)
        {
            var declarations = localDeclarationStatement.Declaration.Variables.Select(variable => Translate(variable, context));
            return $"let {string.Join(", ", declarations)};";
        }

        /// <summary>
        /// Variables assigned to a value.
        /// </summary>
        /// <example>
        /// variable1 = Value1
        /// </example>
        private static string TranslateVariableAssignment(VariableDeclaratorSyntax variableDeclarator, TranslateContext context)
        {
            var equalsValue = Translate(variableDeclarator.Initializer.Value, context);
            return $"{variableDeclarator.Identifier.Text} = {equalsValue}";
        }

        /// <summary>
        /// Assignment using the equal operator.
        /// </summary>
        /// <example>
        /// obj.Property1 = Value1
        /// or
        /// obj[index1] = Value1
        /// </example>
        private static string TranslateAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, TranslateContext context)
        {
            var assignValue = Translate(assignmentExpression.Right, context);
            var assignTo = Translate(assignmentExpression.Left, context);

            if (assignTo == "_")
            {
                // Ignore discard assignments
                return assignValue;
            }

            if (context.ParentExpressionType == ParentExpressionType.ObjectCreation)
            {
                // JS object initializer syntax
                return $"{assignTo}: {assignValue}";
            }

            return $"{assignTo} = {assignValue}";
        }

        /// <summary>
        /// Object creation with initializer.
        /// </summary>
        /// <example>
        /// new SomeClass()
        /// {
        ///     Property1 = Value1,
        ///     Property2 = Value2
        /// }
        /// </example>
        private static string TranslateObjectCreationExpression(ObjectCreationExpressionSyntax objectCreationExpression, TranslateContext context)
            => TranslateObjectCreationInitializersExpressions(objectCreationExpression.Initializer.Expressions, context);

        /// <summary>
        /// Anonymous object creation with initializer.
        /// </summary>
        /// <example>
        /// new
        /// {
        ///     Property1 = Value1,
        ///     Property2 = Value2
        /// }
        /// </example>
        private static string TranslateAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax objectCreationExpression, TranslateContext context)
            => TranslateObjectCreationInitializersExpressions(objectCreationExpression.Initializers, context);

        private static string TranslateObjectCreationInitializersExpressions(IEnumerable<SyntaxNode> initializerExpressions, TranslateContext context)
        { 
            using var switcher = new ParentExpressionTypeSwitcher(context, ParentExpressionType.ObjectCreation);

            var codeLinesBuilder = new IndentedCodeLinesBuilder("  ", ",");
            foreach (var initExpression in initializerExpressions)
            {
                if (initExpression is AssignmentExpressionSyntax assignmentExpression)
                {
                    codeLinesBuilder.Append(TranslateAssignmentExpression(assignmentExpression, context));
                }
                else if (initExpression is AnonymousObjectMemberDeclaratorSyntax anonymousMemberDeclarator)
                {
                    codeLinesBuilder.Append($"{Translate(anonymousMemberDeclarator.NameEquals.Name, context)}: {Translate(anonymousMemberDeclarator.Expression, context)}");
                }
                else
                {
                    throw new InvalidOperationException("Object creation initializer only support member assignment.");
                }
            }

            return $$"""
            {
            {{codeLinesBuilder.Build()}}
            }
            """;
        }

        /// <summary>
        /// Array creation.
        /// </summary>
        /// <example>
        /// new string[] { Value1, Value2, Value3 }
        /// </example>
        private static string TranslateArrayCreationExpression(ArrayCreationExpressionSyntax arrayCreationExpression, TranslateContext context)
            => TranslateArrayCreationInitializerExpressions(arrayCreationExpression.Initializer.Expressions, context);

        /// <summary>
        /// Implicit array creation.
        /// </summary>
        /// <example>
        /// new[] { Value1, Value2, Value3 }
        /// </example>
        private static string TranslateImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression, TranslateContext context)
            => TranslateArrayCreationInitializerExpressions(implicitArrayCreationExpression.Initializer.Expressions, context);

        private static string TranslateArrayCreationInitializerExpressions(IEnumerable<SyntaxNode> initializerExpressions, TranslateContext context)
        {
            var codeLinesBuilder = new IndentedCodeLinesBuilder("  ", ",");
            foreach (var element in initializerExpressions)
            {
                codeLinesBuilder.Append(Translate(element, context));
            }

            return $"""
                [
                {codeLinesBuilder.Build()}
                ]
                """;
        }

        /// <summary>
        /// Collection with elements or spread operator.
        /// </summary>
        /// <example>
        /// [Value1, Value2, Value3] or [.. Collection1, .. Collection2]
        /// </example>
        private static string TranslateCollectionExpression(CollectionExpressionSyntax collectionExpression, TranslateContext context)
        {
            var codeLinesBuilder = new IndentedCodeLinesBuilder("  ", ",");
            foreach (var element in collectionExpression.Elements)
            {
                if (element is ExpressionElementSyntax expressionElement)
                {
                    codeLinesBuilder.Append(Translate(expressionElement.Expression, context));
                }
                else if (element is SpreadElementSyntax spreadElement)
                {
                    codeLinesBuilder.Append("..." + Translate(spreadElement.Expression, context));
                }
                else
                {
                    throw new InvalidOperationException("Collection initializer only support assignment expression.");
                }
            }

            return $"""
                [
                {codeLinesBuilder.Build()}
                ]
                """;
        }

        /// <summary>
        /// Invocation of a method.
        /// </summary>
        /// <example>
        /// obj.Method1(arg1, arg2)
        /// </example>
        private static string TranslateInvocationExpression(InvocationExpressionSyntax invocationExpression, TranslateContext context)
        {
            var args = invocationExpression.ArgumentList.Arguments.Select(arg => Translate(arg, context));
            var invoked = Translate(invocationExpression.Expression, context);
            return $"{invoked}({string.Join(", ", args)})";
        }

        /// <summary>
        /// Access a member of a class/object/namespace.
        /// </summary>
        /// <example>
        /// obj.Property1
        /// or
        /// obj.Method1
        /// </example>
        private static string TranslateMemberAccessExpression(MemberAccessExpressionSyntax memberAccessExpression, TranslateContext context)
        {
            var target = Translate(memberAccessExpression.Expression, context);
            var jsAccessPath = context.GetJsAccessPath(memberAccessExpression, context.GetSymbol(memberAccessExpression));
            if (jsAccessPath is not null && !context.IsJsAccessPath(memberAccessExpression.Expression))
            {
                return jsAccessPath;
            }

            var member = jsAccessPath ?? memberAccessExpression.Name.ToString();
            return $"{target}.{member}";
        }

        /// <summary>
        /// Argument of a method invocation.
        /// </summary>
        private static string TranslateArgument(ArgumentSyntax argument, TranslateContext context)
        {
            // argument of a method invocation
            var type = context.GetType(argument.Expression);
            var translated = Translate(argument.Expression, context);
            if (!context.IsJsAccessPath(argument.Expression) && argument.Expression is IdentifierNameSyntax)
            {
                // Pass to JS during initialization
                var referenceKey = context.AddReference(translated, $"({type.ConvertedType}){translated}");
                return $"fromReference('{referenceKey}')";
            }

            return translated;
        }

        /// <summary>
        /// A named identifier.
        /// </summary>
        private static string TranslateIdentifierName(IdentifierNameSyntax identifierName, TranslateContext context)
        {
            var symbol = context.GetSymbol(identifierName);
            var jsAccessPath = context.GetJsAccessPath(identifierName, symbol);
            if (!string.IsNullOrEmpty(jsAccessPath))
            {
                return jsAccessPath;
            }

            return identifierName.Identifier.Text;
        }

        /// <summary>
        /// An alias qualified name, for example enum value.
        /// </summary>
        private static string TranslateAliasQualifiedName(AliasQualifiedNameSyntax aliasQualifiedName, TranslateContext context)
        {
            var symbol = context.GetSymbol(aliasQualifiedName);
            var jsAccessPath = context.GetJsAccessPath(aliasQualifiedName, symbol);
            if (!string.IsNullOrEmpty(jsAccessPath))
            {
                return jsAccessPath;
            }

            return aliasQualifiedName.Name.Identifier.Text;
        }

        /// <summary>
        /// A literal expression.
        /// </summary>
        /// <example>
        /// "a string value"
        /// or
        /// 123.45
        /// </example>
        private static string TranslateLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax)
        {
            if (literalExpressionSyntax.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                return literalExpressionSyntax.Token.ValueText;
            }
            return literalExpressionSyntax.Token.Text;
        }
    }
}
