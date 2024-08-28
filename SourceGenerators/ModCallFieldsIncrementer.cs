﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
internal class ModCallFieldsIncrementer : IIncrementalGenerator {
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) {
        var modCallMethods = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetGenTarget(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(modCallMethods,
          static (spc, source) => Execute(source, spc));
    }

    static bool IsSyntaxTargetForGeneration(SyntaxNode node) {
        return node is FieldDeclarationSyntax;
    }

    static ModCallInfo? GetGenTarget(GeneratorSyntaxContext context) {
        var field = (FieldDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(field.Declaration.Variables.FirstOrDefault()) is not IFieldSymbol fieldSymbol) {
            return null;
        }

        foreach (AttributeData attr in fieldSymbol.GetAttributes()) {
            string fullName = attr.AttributeClass.ToString();

            if (fullName == ModCallAttribute.FullyQualifiedName) {
                return CollectGenInfo(fieldSymbol, attr);
            }
        }

        return null;
    }

    static ModCallInfo? CollectGenInfo(IFieldSymbol field, AttributeData attr) {
        string name = field.Name;

        try {
            ModCallAttribute.GetAttributeParams(attr, ref name);

            string fieldAccess = $"global::{field.ContainingType}.{field.Name}";

            string returnType = field.Type?.ToString() ?? "void";

            if (returnType == "void") {
                return null;
            }

            return new ModCallInfo(name, returnType, fieldAccess);
        }
        catch (System.Exception ex) {
            return new ModCallInfo($"ERROR_{name.ToUpper()}", "void", ex.ToString());
        }
    }

    static void Execute(ModCallInfo? info, SourceProductionContext context) {
        if (!(info is { } value)) {
            return;
        }

        string result =
        $$"""
            namespace {{Namespace}};

            public partial class ModCallHandler {
                static {{value.FullyQualifiedType}} {{value.Name}}(object[] args) {
                    if (args.Length > 1) {
                        if (args[1] is not {{value.FullyQualifiedType}} value) {
                            throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"{{value.FullyQualifiedType}}\".");
                        }
            
                        {{value.FieldAccess}} = value;
                    }
            
                    return {{value.FieldAccess}};
                }
            }
            """;

        context.AddSource($"ModCalls.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    record struct ModCallInfo(string Name, string FullyQualifiedType, string FieldAccess);
}