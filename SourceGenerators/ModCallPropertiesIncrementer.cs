﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
internal class ModCallPropertiesIncrementer : IIncrementalGenerator {
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
        return node is PropertyDeclarationSyntax;
    }

    static ModCallInfo? GetGenTarget(GeneratorSyntaxContext context) {
        var property = (PropertyDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(property) is not IPropertySymbol propertySymbol) {
            return null;
        }

        foreach (AttributeData attr in propertySymbol.GetAttributes()) {
            string fullName = attr.AttributeClass.ToString();

            if (fullName == ModCallAttribute.FullyQualifiedName) {
                return CollectGenInfo(propertySymbol, attr);
            }
        }

        return null;
    }

    static ModCallInfo? CollectGenInfo(IPropertySymbol prop, AttributeData attr) {
        string name = prop.Name;

        try {
            ModCallAttribute.GetAttributeParams(attr, ref name);

            string propertyAccess = $"global::{prop.ContainingType}.{prop.Name}";

            string returnType = prop.Type?.ToString() ?? "void";

            if (returnType == "void") {
                return null;
            }

            return new ModCallInfo(name, returnType, propertyAccess);
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

            // Test change
            public partial class ModCallHandler {
                static {{value.FullyQualifiedPropType}} {{value.Name}}(object[] args) {
                    if (args.Length > 1) {
                        if (args[1] is not {{value.FullyQualifiedPropType}} value) {
                            throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"{{value.FullyQualifiedPropType}}\".");
                        }
            
                        {{value.PropertyAccess}} = value;
                    }
            
                    return {{value.PropertyAccess}};
                }
            }
            """;

        context.AddSource($"ModCalls.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    record struct ModCallInfo(string Name, string FullyQualifiedPropType, string PropertyAccess);
}